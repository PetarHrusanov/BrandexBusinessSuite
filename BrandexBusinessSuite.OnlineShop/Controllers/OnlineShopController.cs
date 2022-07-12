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
    [Authorize(Roles = $"{AdministratorRoleName}, {AccountantRoleName}")]
    public async Task<IActionResult> ProcessNewOrders()
    {
        
        RestAPI rest = new RestAPI("https://botanic.cc/wp-json/wc/v3",_wooCommerceSettings.Key, _wooCommerceSettings.Secret);
        WCObject wc = new WCObject(rest);

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
            
            // var responseContentJObj = await 
            //     JObjectByUriPostRequest(Client, speedyLink, jsonPostString);
            //
            // if(responseContentJObj.ContainsKey("error")) continue;
            //
            // var speedyTracking = responseContentJObj["id"]!.ToString();
            // var deliveryPrice = Convert.ToDouble(responseContentJObj["price"]!["total"]!.ToString());
            
            // var saleInvoiceCheck = new SaleInvoiceCheck(
            //     $"{order.date_created:yyyy-MM-dd}",
            //     (double)order.total,
            //     order.shipping.first_name+" "+order.shipping.last_name,
            //     order.id.ToString(),
            //     order.shipping.city,
            //     deliveryPrice,
            //     speedyTracking
            //     );

            var erpSale = new ErpOnlineSale(
                "General_DocumentTypes(1f758f75-6b2d-4c0f-a630-09e2aa893348)",
                "General_EnterpriseCompanies(2c186d87-e81d-4318-9a7f-3cfb5399c0d0)",
                "General_Contacts_CompanyLocations(0396623b-ee4e-4a57-867f-5a433d8b6440)",
                "Crm_Customers(6053262d-3544-4cbb-a322-7036c07570ef)",
                "General_Currencies(3187833a-d3c1-4804-bfc0-e17e6aee3069)",
                "Finance_Payments_PaymentAccounts(708814be-0e15-45e3-854a-e2c0235b3231)",
                "Finance_Payments_PaymentTypes(2ae637f8-a1ab-4a47-b3db-3ecbd40b821c)",
                "Crm_Pos_Devices(99dd9d0e-3a74-4ec2-b16d-3c7be3c95ac0)",
                "Crm_Pos_Operators(14adcaae-40b9-40f3-b4dc-94c276804c4c)",
                "Crm_Pos_Terminals(e70d0703-0e2c-4155-890a-a829daf68ef5)",
                "Crm_Pos_Locations(263ed143-67f0-4309-86ff-7bb632fb9730)",
                "Crm_SalesPersons(6a5ca246-d73c-431e-ba5c-460abc9f6af6)",
                "Crm_Customers(e4367b97-b2f3-4bd6-822f-59b8efd1a4c4)",
                "General_Contacts_PartyContactMechanisms(9fad9148-d170-48f9-bcfe-a1ac7ffe44e4)",
                "Logistics_Inventory_Stores(100447ff-44f4-4799-a4c2-7c9b22fb0aaa)",
                order.id.ToString(),
                $"{order.date_created:yyyy-MM-dd}"
                );

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

                decimal discount = Math.Round(((decimal)(1 - productLine.total / productLine.subtotal))!);
                
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
            
            var responseContentJObj = await 
                JObjectByUriPostRequest(Client, "https://brandexbg.my.erp.net/api/domain/odata/Crm_Sales_SalesOrders", jsonPostString);

            if (!responseContentJObj.ContainsKey(ErpDocuments.ODataId)) continue;
            
            var newDocumentId = responseContentJObj[ErpDocuments.ODataId].ToString();
            await ChangeStateToRelease(Client, newDocumentId);

            responseContentJObj = await JObjectByUriGetRequest(Client, $"https://brandexbg.my.erp.net/api/domain/odata/Crm_Sales_SalesOrderLines?$top=20&$filter=SalesOrder%20eq%20'{newDocumentId}'");
            
            var orderLinesList = JsonConvert.DeserializeObject<List<ErpCharacteristicId>>((string)responseContentJObj["value"].ToString());

            var invoiceNew = new ErpInvoice(order.id.ToString());
            

            foreach (var orderLine in orderLinesList)
            {
                responseContentJObj = await JObjectByUriGetRequest(Client, $"https://brandexbg.my.erp.net/api/domain/odata/Crm_Invoicing_InvoiceOrderLines?$top=20&$filter=SalesOrderLine%20eq%20'{orderLine.Id}'");
                var listInvoiceOrderLine = JsonConvert.DeserializeObject<List<ErpInvoiceOrderLines>>((string)responseContentJObj["value"].ToString());

                var invoiceLine = new ErpInvoiceLines()
                {
                    ProductDescription = listInvoiceOrderLine[0].ProductDescription,
                    Quantity = new ErpCharacteristicQuantity(Convert.ToInt16(listInvoiceOrderLine[0].Quantity.Value)),
                    QuantityBase = new ErpCharacteristicQuantity(Convert.ToInt16(listInvoiceOrderLine[0].QuantityBase.Value)),
                    LineAmount = listInvoiceOrderLine[0].LineAmount,
                    UnitPrice = listInvoiceOrderLine[0].UnitPrice,
                    ParentSalesOrderLine = new ErpCharacteristicId(orderLine.Id),
                    SalesOrder = new ErpCharacteristicId(newDocumentId),
                    InvoiceOrderLine = new ErpCharacteristicId(listInvoiceOrderLine[0].Id)
                    
                };
                invoiceNew.Lines.Add(invoiceLine);
            }
            
            jsonPostString = JsonConvert.SerializeObject(invoiceNew, Formatting.Indented);

            responseContentJObj = await 
                JObjectByUriPostRequest(Client, "https://brandexbg.my.erp.net/api/domain/odata/Crm_Invoicing_Invoices/", jsonPostString);
            
            

        }

        return BadRequest();

    }
}