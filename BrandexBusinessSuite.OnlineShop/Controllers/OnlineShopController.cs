namespace BrandexBusinessSuite.OnlineShop.Controllers;

using System.Text;
using System.IO.Compression;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;

using WooCommerceNET;
using WooCommerceNET.WooCommerce.v3;

using BrandexBusinessSuite.Models.ErpDocuments;
using BrandexBusinessSuite.OnlineShop.Data.Models;
using Models.Speedy;
using Services.SalesAnalysis;
using Services.Products;

using BrandexBusinessSuite.Controllers;
using Requests;
using Models;

using static  Common.Constants;
using static Common.ErpConstants;

using static BrandexBusinessSuite.Requests.RequestsMethods;

public class OnlineShopController : ApiController
{
    
    private readonly IWebHostEnvironment _hostEnvironment;
    private readonly UserSettings _userSettings;
    private readonly WooCommerceSettings _wooCommerceSettings;
    
    private readonly IProductsService _productsService;
    private readonly ISalesAnalysisService _salesAnalysisService;

    private static readonly HttpClient Client = new();
    
    public OnlineShopController(IWebHostEnvironment hostEnvironment,
        IOptions<UserSettings> userSettings,
        IOptions<WooCommerceSettings> wooCommerceSettings,
        IProductsService productsService,
        ISalesAnalysisService salesAnalysisService
    )
    {
        _hostEnvironment = hostEnvironment;
        _userSettings = userSettings.Value;
        _wooCommerceSettings = wooCommerceSettings.Value;
        _productsService = productsService;
        _salesAnalysisService = salesAnalysisService;
    }

    [HttpGet]
    [IgnoreAntiforgeryToken]
    [Authorize(Roles = $"{AdministratorRoleName}, {AccountantRoleName}")]
    public async Task<IActionResult> ProcessNewOrders()
    {
        
        var rest = new RestAPI("https://botanic.cc/wp-json/wc/v3",_wooCommerceSettings.Key, _wooCommerceSettings.Secret);
        var wc = new WCObject(rest);

        var orderList = await wc.Order.GetAll(new Dictionary < string, string > ()
        {
            { "status","processing" },
            { "per_page","100" },
        });
        
        var salesInvoicesCheck = new List<SaleInvoiceCheck>();

        var productsDb = await _productsService.GetCheckModels();
        
        foreach (var order in orderList)
        {
            double orderAmountSpeedy = 0;
            if (order.payment_method_title == "Наложен платеж") orderAmountSpeedy = (double)order.total;

            var serviceCodeSpeedy = 2;
            if ( string.Equals(order.shipping.city, "София", StringComparison.OrdinalIgnoreCase)) serviceCodeSpeedy = 113;
            
            var newSpeedyInput = new SpeedyInputOrder(
                _userSettings.UsernameSpeedy,
                _userSettings.PasswordSpeedy,
                new SpeedyInputOrder._Service(serviceCodeSpeedy, orderAmountSpeedy),
                new SpeedyInputOrder._Recipient(order),
                order.id.ToString()
                );
         
            var jsonPostString = JsonConvert.SerializeObject(newSpeedyInput, Formatting.Indented);

            const string speedyLink = "https://api.speedy.bg/v1/shipment";

            var responseContentJObj = await JObjectByUriPostRequest(Client, speedyLink, jsonPostString);
            
            if(responseContentJObj.ContainsKey("error")) continue;
            
            var speedyTracking = responseContentJObj["id"]!.ToString();
            var deliveryPrice = Convert.ToDouble(responseContentJObj["price"]!["total"]!.ToString());
            
            var saleInvoiceCheck = new SaleInvoiceCheck(
                $"{order.date_created:yyyy-MM-dd}",
                (double)order.total,
                order.shipping.first_name+" "+order.shipping.last_name,
                order.id.ToString(),
                order.shipping.city,
                deliveryPrice,
                speedyTracking);

            var erpSale = new ErpOnlineSale(order.id.ToString(), $"{order.date_created:yyyy-MM-dd}");

            foreach (var line in from productLine in order.line_items let productCurrent = productsDb.FirstOrDefault(p => p.WooCommerceName == productLine.name) let discount = Math.Round(((decimal)(1 - productLine.total / productLine.subtotal))!)
                     select new ErpSalesLines(
                         $"General_Products_Products({productCurrent!.ErpCode})",
                         (decimal)productLine.quantity,
                         discount,
                         $"Crm_ProductPrices({productCurrent!.ErpPriceCode})",
                         productCurrent!.ErpPriceNoVat,
                         $"Logistics_Inventory_Lots({productCurrent!.ErpLot})"
                     )) 
            {
                erpSale.Lines.Add(line);
            }

            if (order.shipping_total!=0)
            {
                var line = new ErpSalesLines(
                    $"General_Products_Products(2753534d-23b6-4bde-afa3-889ebc41f18f)",
                    1,
                    0,
                    $"Crm_ProductPrices(48621f6d-7dbe-4995-8c22-e39fc91b7ec5)",
                    (decimal)4.58
                );
                
                erpSale.Lines.Add(line);
            }
            
            jsonPostString = JsonConvert.SerializeObject(erpSale, Formatting.Indented);
            
            var byteArray = Encoding.ASCII.GetBytes($"{_userSettings.UsernameErp}:{_userSettings.PasswordErp}");
            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            
            responseContentJObj = await JObjectByUriPostRequest(Client, "https://brandexbg.my.erp.net/api/domain/odata/Crm_Sales_SalesOrders", jsonPostString);

            if (!responseContentJObj.ContainsKey(ErpDocuments.ODataId)) continue;
            
            var newDocumentId = responseContentJObj[ErpDocuments.ODataId]!.ToString();
            await ChangeStateToRelease(Client, newDocumentId);

            responseContentJObj = await JObjectByUriGetRequest(Client, $"https://brandexbg.my.erp.net/api/domain/odata/Crm_Sales_SalesOrderLines?$top=20&$filter=SalesOrder%20eq%20'{newDocumentId}'");
            
            var orderLinesList = JsonConvert.DeserializeObject<List<ErpSalesLinesOutput>>(responseContentJObj["value"].ToString());

            var invoiceNew = new ErpInvoice(order.id.ToString(), $"{order.date_created:yyyy-MM-dd}");

            foreach (var orderLine in orderLinesList)
            {
                responseContentJObj = await JObjectByUriGetRequest(Client, $"https://brandexbg.my.erp.net/api/domain/odata/Crm_Invoicing_InvoiceOrderLines?$top=20&$filter=SalesOrderLine%20eq%20'{orderLine.Id}'");
                var listInvoiceOrderLine = JsonConvert.DeserializeObject<List<ErpInvoiceOrderLines>>(responseContentJObj["value"].ToString());

                var invoiceLine = new ErpInvoiceLines(listInvoiceOrderLine![0], orderLine, newDocumentId);
                
                invoiceNew.Lines.Add(invoiceLine);
            }

            jsonPostString = JsonConvert.SerializeObject(invoiceNew, Formatting.Indented);

            responseContentJObj = await JObjectByUriPostRequest(Client, "https://brandexbg.my.erp.net/api/domain/odata/Crm_Invoicing_Invoices/", jsonPostString);
            
            if (!responseContentJObj.ContainsKey(ErpDocuments.ODataId)) continue;
            
            var newInvoiceId = responseContentJObj[ErpDocuments.ODataId].ToString();
            var newInvoiceNo = responseContentJObj[ErpDocuments.DocumentNo].ToString();

            saleInvoiceCheck.InvoiceNumber = newInvoiceNo;
            
            await ChangeStateToRelease(Client, newInvoiceId);
            
            salesInvoicesCheck.Add(saleInvoiceCheck);

        }

        return BadRequest("Kur");
    }

    [HttpGet]
    [IgnoreAntiforgeryToken]
    [Authorize(Roles = $"{AdministratorRoleName}, {AccountantRoleName}")]
    public async Task<IActionResult> Speedy()
    {
        var request = new SpeedyPrintRequest(_userSettings.UsernameSpeedy, _userSettings.PasswordSpeedy);

        string[] testko = { "61828714942", "61828606150" };

        foreach (var variable in testko)
        {
            request.Parcels.Add(new SpeedyParcelId(variable));
        }
        
        var jsonPostString = JsonConvert.SerializeObject(request, Formatting.Indented);
        
        var uri = new Uri("https://api.speedy.bg/v1/print");
        var content = new StringContent(jsonPostString, Encoding.UTF8, "application/json");
        var response = await Client.PostAsync(uri, content);
        var responseContent = response.Content;

        var sWebRootFolder = _hostEnvironment.WebRootPath;
        
        var newPath = Path.Combine(sWebRootFolder, DateTime.Now.ToString("yyyy-MM-dd:HH:mm:ss"));
        
        if (!Directory.Exists(newPath))
        {
            Directory.CreateDirectory(newPath);
        }
        
        const string sFileName = "Kur.pdf";

        await using(var newFile = System.IO.File.Create(Path.Combine(newPath,sFileName )))
        { 
            var stream = await responseContent.ReadAsStreamAsync();
            await stream.CopyToAsync(newFile);
        }
        
        var files = Directory.GetFiles(newPath);
        
        var zipFile = Path.Combine(newPath, "Kuromir.zip");
        
        using (var archive = ZipFile.Open(zipFile, ZipArchiveMode.Create))
        {
            foreach (var fPath in files)
            {
                archive.CreateEntryFromFile(fPath, Path.GetFileName(fPath));
            }
        }
        
        var memory = new MemoryStream();

        await using (var stream = new FileStream(zipFile, FileMode.Open))
        {
            await stream.CopyToAsync(memory);
        }

        memory.Position = 0;

        // return File(memory, "application/pdf", sFileName);
        
        return File(memory, "application/zip", "Kuromir.zip");

    }
    
    [HttpGet]
    [IgnoreAntiforgeryToken]
    [Authorize(Roles = $"{AdministratorRoleName}, {AccountantRoleName}")]
    public async Task<IActionResult> OrdersWooCommerceCheck()
    {
        var rest = new RestAPI("https://botanic.cc/wp-json/wc/v3",_wooCommerceSettings.Key, _wooCommerceSettings.Secret);
        var wc = new WCObject(rest);

        var dateInput = "Jul 19, 2022";
        var parsedDate = DateTime.Parse(dateInput);
        
        var orderList = await wc.Order.GetAll(new Dictionary < string, string > ()
        {
            { "date_created",parsedDate.ToString() },
        });
        
        var productsDb = await _productsService.GetCheckModels();

        var ordersForAnalysis = new List<SalesOnlineAnalysisInput>();

        foreach (var order in orderList)
        {

            var sample = order.meta_data.Where(p => p.key == "sample").Select(p => p.value).FirstOrDefault();
            var adSource = order.meta_data.Where(p => p.key == "order_details_information_source").Select(p => p.value).FirstOrDefault();
            
            foreach (var orderLine in order.line_items)
            {
                var orderAnalysis = new SalesOnlineAnalysisInput
                {
                    OrderNumber = order.number,
                    Date = order.date_created,
                    ProductId = productsDb.Where(p => p.WooCommerceName == orderLine.name).Select(p => p.Id)
                        .FirstOrDefault(),
                    Quantity = orderLine.quantity,
                    Total = orderLine.total,
                    City = order.shipping.city,
                    Sample = (string)sample,
                    AdSource = (string)adSource
                };
                ordersForAnalysis.Add(orderAnalysis);

            }
        }

        // var ordersForAnalysis = (
        //     from order in orderList 
        //     from orderLine in order.line_items 
        //     select new SalesOnlineAnalysisInput 
        //     { 
        //         OrderNumber = order.number, 
        //         Date = order.date_created, 
        //         ProductId = productsDb.Where(p => p.WooCommerceName == orderLine.name).Select(p => p.Id).FirstOrDefault(), 
        //         Quantity = orderLine.quantity, 
        //         Total = orderLine.total, 
        //         City = order.shipping.city, 
        //         Sample = order.meta_data["sample"], 
        //         AdSource = "Kur" 
        //     }).ToList();

        await _salesAnalysisService.UploadBulk(ordersForAnalysis);

        return BadRequest("Tasho");

    }

}