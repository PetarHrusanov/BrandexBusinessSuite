using BrandexBusinessSuite.OnlineShop.Data.Models;

namespace BrandexBusinessSuite.OnlineShop.Controllers;

using System.Text;
using System.IO.Compression;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

using WooCommerceNET;
using WooCommerceNET.Base;
using WooCommerceNET.WooCommerce.v3;

using BrandexBusinessSuite.Models.ErpDocuments;
using BrandexBusinessSuite.Services;
using BrandexBusinessSuite.Controllers;
using Models.Speedy;
using Services.SalesAnalysis;
using Services.Products;
using Services.DeliveryPrices;
using Requests;
using Models;

using static  Common.Constants;
using static Common.ErpConstants;

using static BrandexBusinessSuite.Requests.RequestsMethods;

public class OnlineShopController : ApiController
{
    
    private readonly IWebHostEnvironment _hostEnvironment;
    private readonly SpeedyUserSettings _speedyUserSettings;
    private readonly ErpUserSettings _erpUserSettings;
    private readonly WooCommerceSettings _wooCommerceSettings;
    
    private readonly IProductsService _productsService;
    private readonly ISalesAnalysisService _salesAnalysisService;
    private readonly IDeliveryPriceService _deliveryPriceService;

    private static readonly HttpClient Client = new();
    
    public OnlineShopController(IWebHostEnvironment hostEnvironment,
        IOptions<SpeedyUserSettings> speedyUserSettings,
        IOptions<ErpUserSettings> erpUserSettings,
        IOptions<WooCommerceSettings> wooCommerceSettings,
        IProductsService productsService,
        ISalesAnalysisService salesAnalysisService,
        IDeliveryPriceService deliveryPriceService
    )
    {
        _hostEnvironment = hostEnvironment;
        _speedyUserSettings = speedyUserSettings.Value;
        _erpUserSettings = erpUserSettings.Value;
        _wooCommerceSettings = wooCommerceSettings.Value;
        _productsService = productsService;
        _salesAnalysisService = salesAnalysisService;
        _deliveryPriceService = deliveryPriceService;
    }

    [HttpGet]
    [Authorize(Roles = $"{AdministratorRoleName}, {AccountantRoleName}")]
    public async Task<DeliveryPrice> GetDeliveryPrice() 
        => await _deliveryPriceService.GetDeliveryPrice();

    [HttpPost]
    [Authorize(Roles = $"{AdministratorRoleName}, {AccountantRoleName}")]
    public async Task<ActionResult> EditDeliveryPrice(DeliveryPrice deliveryPrice)
    { 
        await _deliveryPriceService.EditDeliveryPrice(deliveryPrice);
        return Result.Success;
    }

    [HttpGet]
    [IgnoreAntiforgeryToken]
    [Authorize(Roles = $"{AdministratorRoleName}, {AccountantRoleName}")]
    public async Task<ActionResult> CheckBatches()
    {
        var productsDb = await _productsService.GetCheckModels();
        
        var byteArray = Encoding.ASCII.GetBytes($"{_erpUserSettings.User}:{_erpUserSettings.Password}");
        Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

        foreach (var product in productsDb)
        {
            var responseContentJObj = await JObjectByUriGetRequest(Client,
                $"{ErpRequests.BaseUrl}Logistics_Inventory_Lots?$top=1000&$filter=Product%20eq%20'General_Products_Products({product.ErpCode})'");
            var batchesList = JsonConvert.DeserializeObject<List<ErpLot>>(responseContentJObj["value"].ToString());

            var newestBatch = batchesList!.OrderByDescending(p=>p.ExpiryDate).ThenByDescending(p=>p.ReceiptDate).FirstOrDefault();

            if (product.ErpLot == newestBatch!.Id) continue;
            
            product.ErpLot = newestBatch.Id;
            await _productsService.UpdateProduct(product);
        }
        
        return Result.Success;
    }

    [HttpGet]
    [IgnoreAntiforgeryToken]
    [Authorize(Roles = $"{AdministratorRoleName}, {AccountantRoleName}")]
    public async Task<ActionResult> ProcessNewOrders()
    {
        
        var rest = new RestAPI("https://botanic.cc/wp-json/wc/v3",_wooCommerceSettings.Key, _wooCommerceSettings.Secret);
        var wc = new WCObject(rest);

        var orderList = await wc.Order.GetAll(new Dictionary < string, string > ()
        {
            { "status","processing" },
            { "per_page","40" },
        });
        var productsDb = await _productsService.GetCheckModels();
        var productsDict = productsDb.ToDictionary(p => p.WooCommerceName);
        var delivery = await _deliveryPriceService.GetDeliveryPrice();

        var salesInvoicesCheck = new List<SaleInvoiceCheck>();
        var speedyTrackingList = new List<string>();
        
        foreach (var order in orderList)
        {
            
            double orderAmountSpeedy = 0;
            if (order.payment_method_title == "Наложен платеж") orderAmountSpeedy = (double)order.total!;

            var newSpeedyInput = new SpeedyInputOrder(
                _speedyUserSettings.UsernameSpeedy,
                _speedyUserSettings.PasswordSpeedy,
                orderAmountSpeedy,
                order
            );
         
            var jsonPostString = JsonConvert.SerializeObject(newSpeedyInput, Formatting.Indented);

            const string speedyLink = "https://api.speedy.bg/v1/shipment";

            JObject responseContentJObj;
            
            try
            {
                responseContentJObj = await JObjectByUriPostRequest(Client, speedyLink, jsonPostString);
                if(responseContentJObj.ContainsKey("error")) continue;
            }
            catch (Exception e)
            {
                continue;
            }

            var speedyTracking = responseContentJObj["id"]!.ToString();
            speedyTrackingList.Add(speedyTracking);
            var deliveryPrice = Convert.ToDouble(responseContentJObj["price"]!["total"]!.ToString());

            var saleInvoiceCheck = new SaleInvoiceCheck(order, deliveryPrice, speedyTracking);
            
            if (order.payment_method_title != "Наложен платеж") saleInvoiceCheck.Notes = "платена";
            
            salesInvoicesCheck.Add(saleInvoiceCheck);

            var erpSale = new ErpOnlineSale(order.id.ToString()!, $"{order.date_created:yyyy-MM-dd}");

            var lines = order.line_items.Select(productLine =>
            {
                var productCurrent = productsDict[productLine.name];
                var discount = Math.Round((decimal)(1 - productLine.total! / productLine.subtotal!), 2);
                return new ErpSalesLines(
                    $"General_Products_Products({productCurrent.ErpCode})",
                    (decimal)productLine.quantity,
                    discount,
                    $"Crm_ProductPrices({productCurrent.ErpPriceCode})",
                    productCurrent.ErpPriceNoVat,
                    $"Logistics_Inventory_Lots({productCurrent.ErpLot})"
                );
            });

            erpSale.Lines.AddRange(lines);

            if (order.shipping_total!=0)
            {
                var line = new ErpSalesLines(
                    $"General_Products_Products({delivery.ErpId})",
                    1,
                    0,
                    $"Crm_ProductPrices({delivery.ErpPriceId})",
                    delivery.Price
                );
                
                erpSale.Lines.Add(line);
            }

            var sample = order.meta_data.Where(p => p.key == "sample").Select(p => p.value).FirstOrDefault();

            if (sample!=null && (string)sample != "няма")
            {
                var sampleCode = productsDb.Where(s => s.WooCommerceSampleName == (string)sample).Select(s => s.ErpSampleCode).FirstOrDefault();
                
                var line = new ErpSalesLines(
                    $"General_Products_Products({sampleCode})",
                    1,
                    0,
                    $"Crm_ProductPrices(48621f6d-7dbe-4995-8c22-e39fc91b7ec5)",
                    (decimal)0.00
                );
                
                erpSale.Lines.Add(line);
            }
            
            jsonPostString = JsonConvert.SerializeObject(erpSale, Formatting.Indented);

            AuthenticateUserBasicHeader(Client, _erpUserSettings.User, _erpUserSettings.Password);

            try
            {
                responseContentJObj = await JObjectByUriPostRequest(Client, "https://brandexbg.my.erp.net/api/domain/odata/Crm_Sales_SalesOrders", jsonPostString);
            }
            catch
            {
                continue;
            }

            if (!responseContentJObj.ContainsKey(ErpDocuments.ODataId)) continue;
            
            var newDocumentId = responseContentJObj[ErpDocuments.ODataId]!.ToString();

            try
            {
                await ChangeStateToRelease(Client, newDocumentId);
                responseContentJObj = await JObjectByUriGetRequest(Client, $"https://brandexbg.my.erp.net/api/domain/odata/Crm_Sales_SalesOrderLines?$top=20&$filter=SalesOrder%20eq%20'{newDocumentId}'");
            }
            catch
            {
                continue;
            }

            var orderLinesList = JsonConvert.DeserializeObject<List<ErpSalesLinesOutput>>(responseContentJObj["value"].ToString());

            var invoiceNew = new ErpInvoice(order.id.ToString()!, $"{order.date_created:yyyy-MM-dd}");

            try
            {
                responseContentJObj = await JObjectByUriGetRequest(Client, $"{ErpRequests.BaseUrl}Crm_Invoicing_InvoiceOrders?$top=100&$filter=SalesOrder%20eq%20'{newDocumentId}'&$select=Id&$expand=Lines($expand=SalesOrderLine($select=Id);$select=Id,LineAmount,LineCustomDiscountPercent,ProductDescription,Quantity,QuantityBase,UnitPrice)");
            }
            catch
            {
                continue;
            }

            var invoiceOrders = JsonConvert.DeserializeObject<List<ErpInvoiceOrder>>(responseContentJObj["value"]!.ToString());
            var invoiceOrderLines = invoiceOrders!.SelectMany(x => x.Lines);
            var invoiceLines = from orderLine in orderLinesList
                join invoiceOrderLine in invoiceOrderLines on orderLine.Id equals invoiceOrderLine.SalesOrderLine.Id
                select new ErpInvoiceLines(invoiceOrderLine, orderLine, newDocumentId);
        
            invoiceNew.Lines.AddRange(invoiceLines);
            
            if (invoiceNew.Lines.Count == 0) continue;

            jsonPostString = JsonConvert.SerializeObject(invoiceNew, Formatting.Indented);
            try
            {
                responseContentJObj = await JObjectByUriPostRequest(Client, $"{ErpRequests.BaseUrl}Crm_Invoicing_Invoices/", jsonPostString);
            }
            catch
            {
                continue;
            }
            
            var newInvoiceId = responseContentJObj.GetValue(ErpDocuments.ODataId)?.ToString() ?? "";
            if (newInvoiceId == "") continue;
            
            saleInvoiceCheck.InvoiceNumber = responseContentJObj[ErpDocuments.DocumentNo]!.ToString();
            try
            {
                await ChangeStateToRelease(Client, newInvoiceId);
            }
            catch
            {
                // ignored
            }
        }
        
        try
        {
            var orders = salesInvoicesCheck.Select(p => new Order()
            {
                id = (ulong?)int.Parse(p.Order),
                status = "shipped"
            }).ToList();
            var batch = new BatchObject<Order>
            {
                update = orders
            };
            await wc.Order.UpdateRange(batch);
        }
        catch
        {
            // ignored
        }
        
        var speedyPrintRequest = new SpeedyPrintRequest(_speedyUserSettings.UsernameSpeedy, _speedyUserSettings.PasswordSpeedy);
        speedyPrintRequest.Parcels.AddRange(speedyTrackingList.Select(code => new SpeedyParcelId(code)));

        var speedyContentSerialized = JsonConvert.SerializeObject(speedyPrintRequest, Formatting.Indented);
        
        var uri = new Uri("https://api.speedy.bg/v1/print");
        var content = new StringContent(speedyContentSerialized, Encoding.UTF8, "application/json");
        
        var response = await ExecuteWithRetry(uri, content);
        var responseContent = response.Content;

        var sWebRootFolder = _hostEnvironment.WebRootPath;
        
        var newPath = Path.Combine(sWebRootFolder, DateTime.Now.ToString("yyyy-MM-dd:HH:mm:ss"));
        
        if (!Directory.Exists(newPath)) Directory.CreateDirectory(newPath);
        
        await using(var newFile = System.IO.File.Create(Path.Combine(newPath,"tracking_codes.pdf" )))
        { 
            var stream = await responseContent.ReadAsStreamAsync();
            await stream.CopyToAsync(newFile);
        }
        
        await using (var fs = new FileStream(Path.Combine(newPath, "invoices.xlsx"), FileMode.Create, FileAccess.Write))
        {
            IWorkbook workbook = new XSSFWorkbook();

            var excelSheet = workbook.CreateSheet("invoice_info");
            var row = excelSheet.CreateRow(0);

            var titlesArray = new[] { "Фактура", "Дата", "Стойност", "Клиент", "№ Поръчка", 
                "Град","Цена доставка","Товарителница",  "Бележки"};

            foreach (var title in titlesArray) row.CreateCell(row.Cells.Count).SetCellValue(title);

            foreach (var sale in salesInvoicesCheck)
            {
                row = excelSheet.CreateRow(excelSheet.LastRowNum + 1);
                row.CreateCell(row.Cells.Count).SetCellValue(sale.InvoiceNumber);
                row.CreateCell(row.Cells.Count).SetCellValue(sale.Date);
                row.CreateCell(row.Cells.Count).SetCellValue(sale.OrderTotal);
                row.CreateCell(row.Cells.Count).SetCellValue(sale.ClientName);
                row.CreateCell(row.Cells.Count).SetCellValue(sale.Order);
                row.CreateCell(row.Cells.Count).SetCellValue(sale.City);
                row.CreateCell(row.Cells.Count).SetCellValue(sale.DeliveryPrice);
                row.CreateCell(row.Cells.Count).SetCellValue(sale.TrackingCode);
                row.CreateCell(row.Cells.Count).SetCellValue(sale.Notes);
            }
            
            workbook.Write(fs);
        }
        
        // var files = Directory.GetFiles(newPath);
        //
        // var zipFile = Path.Combine(newPath, "Collection.zip");
        //
        // using (var archive = ZipFile.Open(zipFile, ZipArchiveMode.Create))
        // {
        //     foreach (var fPath in files)
        //     {
        //         archive.CreateEntryFromFile(fPath, Path.GetFileName(fPath));
        //     }
        // }
        //
        // var memory = new MemoryStream();
        //
        // await using (var stream = new FileStream(zipFile, FileMode.Open))
        // {
        //     await stream.CopyToAsync(memory);
        // }
        //
        // memory.Position = 0;
        //
        // return File(memory, "application/zip", "Collection.zip");
        
        var files = Directory.GetFiles(newPath);
        var memory = new MemoryStream();
        using (var archive = new ZipArchive(memory, ZipArchiveMode.Create, true))
        {
            foreach (var fPath in files)
            {
                archive.CreateEntryFromFile(fPath, Path.GetFileName(fPath));
            }
        }
        memory.Position = 0;
        return File(memory, "application/zip", "Collection.zip");
    }

    [HttpPost]
    [IgnoreAntiforgeryToken]
    [Authorize(Roles = $"{AdministratorRoleName}, {AccountantRoleName}")]
    public async Task<ActionResult> OrdersWooCommerceCheck([FromBody] DateInput date)
    {
        var rest = new RestAPI("https://botanic.cc/wp-json/wc/v3",_wooCommerceSettings.Key, _wooCommerceSettings.Secret);
        var wc = new WCObject(rest);

        var dateFormat = date.Date.ToString("yyyy-MM-dd'T'HH:mm:ss");

        var orderList = await wc.Order.GetAll(new Dictionary < string, string > ()
        {
            { "before", dateFormat},
            { "status","shipped,completed,processing"},
            { "per_page","100" },
        });

        var ordersDatabase = await _salesAnalysisService.GetCheckModelsByDate(date.Date);

        orderList.RemoveAll(x=> ordersDatabase.Any(y=>y.OrderNumber==x.number));

        var productsDb = await _productsService.GetCheckModels();
        
        var ordersForAnalysis = (from order in orderList
            let sample = order.meta_data.Where(p => p.key == "sample").Select(p => p.value).FirstOrDefault()
            let adSource =
                order.meta_data.Where(p => p.key == "order_details_information_source")
                    .Select(p => p.value)
                    .FirstOrDefault()
            from orderLine in order.line_items
            select new SalesOnlineAnalysisInput
            {
                OrderNumber = order.number,
                Date = order.date_created,
                ProductId = productsDb.Where(p => p.WooCommerceName == orderLine.name)
                    .Select(p => p.Id)
                    .FirstOrDefault(),
                Quantity = orderLine.quantity,
                Total = orderLine.total,
                City = order.shipping.city,
                Sample = (string)sample,
                AdSource = (string)adSource
            }).ToList();
        
        await _salesAnalysisService.UploadBulk(ordersForAnalysis);

        return Result.Success;
    }
    
    [HttpGet]
    [IgnoreAntiforgeryToken]
    [Authorize(Roles = $"{AdministratorRoleName}")]
    public async Task<ActionResult> OrdersCompleted()
    {
        var rest = new RestAPI("https://botanic.cc/wp-json/wc/v3",_wooCommerceSettings.Key, _wooCommerceSettings.Secret);
        var wc = new WCObject(rest);
        
        var orderList = await wc.Order.GetAll(new Dictionary < string, string > ()
        {
            { "status","shipped"},
            { "per_page","100" },
        });

        var orderIds = (from order in orderList select new SpeedyReference(order.id.ToString())).ToList();

        var speedyRequest = new SpeedyCheckCompleted(_speedyUserSettings.UsernameSpeedy, _speedyUserSettings.PasswordSpeedy, orderIds);
        var jsonPostString = JsonConvert.SerializeObject(speedyRequest, Formatting.Indented);

        const string speedyLink = "https://api.speedy.bg/v1/track";
        var responseContentJObj = await JObjectByUriPostRequest(Client, speedyLink, jsonPostString);
        
        var parcels = JsonConvert.DeserializeObject<List<SpeedyCheckCompletedResponse>>(responseContentJObj["parcels"].ToString());

        var parcelsCompleted = parcels.Where(o => o.Operations.Any(op => op.OperationCode == -14))
            .Select(r => r.Reference).ToList();

        var orders = parcelsCompleted.Select(p => new Order
        {
            id = (ulong?)int.Parse(p),
            status = "completed"
        }).ToList();
        
        var batch = new BatchObject<Order> { update = orders };
        await wc.Order.UpdateRange(batch);

        return Result.Success;
    }
    
    [HttpGet]
    [IgnoreAntiforgeryToken]
    [Authorize(Roles = $"{AdministratorRoleName}")]
    public async Task<ActionResult> UpdatePrices()
    {
        
        var productsDb = await _productsService.GetCheckModels();
        
        AuthenticateUserBasicHeader(Client, _erpUserSettings.User, _erpUserSettings.Password);

        var queryDate =
            $"Crm_ProductPrices?$top=1000&$filter=PriceList%20eq%20'Crm_PriceLists(db6b7ec3-f3f0-4e1e-811d-5d8e9895843e)'&$select=FromDate,Id,Price,PriceList,Product&$expand=PriceList($select=Id),Product($select=Id)";
        
        var responseContentJObj = await JObjectByUriGetRequest(Client, $"{ErpRequests.BaseUrl}{queryDate}");
        var prices = JsonConvert.DeserializeObject<List<CrmProductPrice>>(responseContentJObj["value"]?.ToString() ?? throw new InvalidOperationException("No result for the request"));

        foreach (var product in productsDb)
        {
            var productPrice = prices!.Where(p => p.Product.Id == product.ErpCode).MaxBy(r => r.FromDate);
            if (product.ErpPriceCode == productPrice.Id) continue;
            product.ErpPriceCode = productPrice.Id;
            product.ErpPriceNoVat = productPrice.Price.Value;
            await _productsService.UpdateProduct(product);
        }

        return Result.Success;
    }

    private static async Task<HttpResponseMessage> ExecuteWithRetry(Uri url, StringContent contentString)
    {
        var result = new HttpResponseMessage();
        var success = false;
        using var client = new HttpClient();
        while (!success)
        {
            result = await client.PostAsync(url, contentString);
            success = result.IsSuccessStatusCode;
        }
        return result;
    } 
}