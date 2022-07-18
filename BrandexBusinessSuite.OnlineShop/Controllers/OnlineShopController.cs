namespace BrandexBusinessSuite.OnlineShop.Controllers;

using System.Reflection;
using System.Text;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;

using WooCommerceNET;
using WooCommerceNET.WooCommerce.v3;

using BrandexBusinessSuite.Models.ErpDocuments;
using BrandexBusinessSuite.OnlineShop.Data.Models;

using BrandexBusinessSuite.Controllers;
using Requests;
using Models;

using static  Common.Constants;
using static Common.ProductConstants;
using static Common.ErpConstants;

using static BrandexBusinessSuite.Requests.RequestsMethods;
using static Methods.FieldsValuesMethods;

public class OnlineShopController : ApiController
{
    
    private readonly IWebHostEnvironment _hostEnvironment;
    private readonly UserSettings _userSettings;

    private readonly WooCommerceSettings _wooCommerceSettings;

    private static readonly HttpClient Client = new();
    
    public OnlineShopController(IWebHostEnvironment hostEnvironment,
        IOptions<UserSettings> userSettings,
        IOptions<WooCommerceSettings> wooCommerceSettings
    )
    {
        _hostEnvironment = hostEnvironment;
        _userSettings = userSettings.Value;
        _wooCommerceSettings = wooCommerceSettings.Value;
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
        
        foreach (var order in orderList)
        {

            double orderAmountSpeedy = 0;
            if (order.payment_method_title == "Наложен платеж") orderAmountSpeedy = (double)order.total;

            int serviceCodeSpeedy = 2;
            if ( string.Equals(order.shipping.city, "София", StringComparison.OrdinalIgnoreCase)) serviceCodeSpeedy = 113;
            
            var newSpeedyInput = new SpeedyInputOrder(
                _userSettings.UsernameSpeedy,
                _userSettings.PasswordSpeedy,
                new SpeedyInputOrder._Service(serviceCodeSpeedy, orderAmountSpeedy),
                new SpeedyInputOrder._Recipient(order.billing.phone,
                    order.shipping.first_name+" "+order.shipping.last_name,
                    order.shipping.city,
                    order.shipping.postcode,
                    order.shipping.address_1),
                order.id.ToString()
                );
         
            var jsonPostString = JsonConvert.SerializeObject(newSpeedyInput, Formatting.Indented);

            var speedyLink = "https://api.speedy.bg/v1/shipment";
            
            var responseContentJObj = await 
                JObjectByUriPostRequest(Client, speedyLink, jsonPostString);
            
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
                speedyTracking
                );

            var erpSale = new ErpOnlineSale(order.id.ToString(), $"{order.date_created:yyyy-MM-dd}");

            foreach (var productLine in order.line_items)
            {
                var productName = typeof(OnlineShop)
                        .GetFields(BindingFlags.Public | BindingFlags.Static)
                        .Where(f => (string)f.GetValue(null)! == productLine.name)
                        .Select(n => n.Name)
                        .FirstOrDefault()
                    ;

                var productCode = ReturnValueByClassAndName(typeof(ErpCodes), productName);
                var productPriceCode = ReturnValueByClassAndName(typeof(ErpPriceCodes), productName);
                var productLotCode = ReturnValueByClassAndName(typeof(ErpLots), productName);
                
                var field = typeof(ErpPriceNoVat).GetField(productName, BindingFlags.Public | BindingFlags.Static);
                var productPriceNotVat =  (decimal)field!.GetValue(null)!;

                var discount = Math.Round(((decimal)(1 - productLine.total / productLine.subtotal))!);
                
                var line = new ErpSalesLines(
                    $"General_Products_Products({productCode})",
                    (decimal)productLine.quantity,
                    discount,
                    $"Crm_ProductPrices({productPriceCode})",
                    productPriceNotVat,
                    $"Logistics_Inventory_Lots({productLotCode})"
                );
                
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
            
            var newDocumentId = responseContentJObj[ErpDocuments.ODataId].ToString();
            await ChangeStateToRelease(Client, newDocumentId);

            responseContentJObj = await JObjectByUriGetRequest(Client, $"https://brandexbg.my.erp.net/api/domain/odata/Crm_Sales_SalesOrderLines?$top=20&$filter=SalesOrder%20eq%20'{newDocumentId}'");
            
            var orderLinesList = JsonConvert.DeserializeObject<List<ErpSalesLinesOutput>>((string)responseContentJObj["value"].ToString());
            
            // var orderLinesList = JsonConvert.DeserializeObject<List<ErpCharacteristicId>>((string)responseContentJObj["value"].ToString());

            var invoiceNew = new ErpInvoice(order.id.ToString(), $"{order.date_created:yyyy-MM-dd}");

            foreach (var orderLine in orderLinesList)
            {
                responseContentJObj = await JObjectByUriGetRequest(Client, $"https://brandexbg.my.erp.net/api/domain/odata/Crm_Invoicing_InvoiceOrderLines?$top=20&$filter=SalesOrderLine%20eq%20'{orderLine.Id}'");
                var listInvoiceOrderLine = JsonConvert.DeserializeObject<List<ErpInvoiceOrderLines>>((string)responseContentJObj["value"].ToString());
                
                var invoiceLine = new ErpInvoiceLines
                {
                    ProductDescription = listInvoiceOrderLine[0].ProductDescription,
                    Quantity = new ErpCharacteristicQuantity(Convert.ToInt16(listInvoiceOrderLine[0].Quantity.Value)),
                    QuantityBase = new ErpCharacteristicQuantity(Convert.ToInt16(listInvoiceOrderLine[0].QuantityBase.Value)),
                    StandardQuantityBase  = new ErpCharacteristicQuantity(Convert.ToInt16(listInvoiceOrderLine[0].QuantityBase.Value)),
                    LineAmount = listInvoiceOrderLine[0].LineAmount,
                    SalesOrderAmount = listInvoiceOrderLine[0].LineAmount.Value,
                    UnitPrice = listInvoiceOrderLine[0].UnitPrice,
                    ParentSalesOrderLine = new ErpCharacteristicId(orderLine.Id),
                    SalesOrder = new ErpCharacteristicId(newDocumentId),
                    InvoiceOrderLine = new ErpCharacteristicId(listInvoiceOrderLine[0].Id),
                    LineNo = orderLine.LineNo,
                    Product = orderLine.Product
                };
                invoiceNew.Lines.Add(invoiceLine);
            }

            jsonPostString = JsonConvert.SerializeObject(invoiceNew, Formatting.Indented);

            responseContentJObj = await 
                JObjectByUriPostRequest(Client, "https://brandexbg.my.erp.net/api/domain/odata/Crm_Invoicing_Invoices/", jsonPostString);
            
            if (!responseContentJObj.ContainsKey(ErpDocuments.ODataId)) continue;
            
            var newInvoiceId = responseContentJObj[ErpDocuments.ODataId].ToString();
            
            await ChangeStateToRelease(Client, newInvoiceId);

            var uri = new Uri($"https://brandexbg.my.erp.net/api/domain/odata/{newInvoiceId}/GetPrintout");

            var response = await Client.PostAsync(uri, null);

            var responsePds = await response.Content.ReadAsStringAsync();
            
            Byte[] bytes = Convert.FromBase64String(responsePds);
            
            var sWebRootFolder = _hostEnvironment.WebRootPath;

            var newPath = Path.Combine(sWebRootFolder, "HUI");
        
            if (!Directory.Exists(newPath))
            {
                Directory.CreateDirectory(newPath);
            }
            
            const string sFileName = @"Print.pdf";
            
            // await System.IO.File.WriteAllBytesAsync(sWebRootFolder, bytes);

            var memory = new MemoryStream();
            
            await using (var stream = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Create))
            {
                stream.Write(bytes, 0, bytes.Length);
            }
            
            await using (var stream = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }

            memory.Position = 0;

            return File(memory, "application/pdf", sFileName);
        }

        return BadRequest();

    }
}