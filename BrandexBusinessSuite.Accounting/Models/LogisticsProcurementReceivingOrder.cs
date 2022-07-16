namespace BrandexBusinessSuite.Accounting.Models;

using BrandexBusinessSuite.Models.ErpDocuments;
using Newtonsoft.Json;

public class LogisticsProcurementReceivingOrder : ErpDocumentSale
{

    public LogisticsProcurementReceivingOrder(string facebookInvoiceNumber, DateTime date)
    {
        DocumentType = new ErpCharacteristicId("General_DocumentTypes(b1787109-6d5e-41ad-8ba6-b9a15ebccf5e)");
        DocumentNo = facebookInvoiceNumber;
        InvoiceDocumentNo = facebookInvoiceNumber;
        EnterpriseCompany = new ErpCharacteristicId("General_EnterpriseCompanies(2c186d87-e81d-4318-9a7f-3cfb5399c0d0)");
        EnterpriseCompanyLocation = new ErpCharacteristicId("General_Contacts_CompanyLocations(f3156c3c-7c04-4de7-bf03-8b983aada49f)");
        FromParty = new ErpCharacteristicId("General_Contacts_Parties(42bef242-101f-48bd-b6c5-8da6819c844f)");
        ToParty = new ErpCharacteristicId("General_Contacts_Parties(b21c6bc3-a4d8-43b9-a3df-b2d39ddf552f)");
        DocumentCurrency = new ErpCharacteristicId("General_Currencies(3187833a-d3c1-4804-bfc0-e17e6aee3069)");
        PaymentAccount = new ErpCharacteristicId("Finance_Payments_PaymentAccounts(b6d37a6d-2ac7-4a9c-a067-edf518bac68d)");
        PaymentType = new ErpCharacteristicId("Finance_Payments_PaymentTypes(7dd31560-4953-4d41-b7e6-3e831fdf8549)");
        DocumentDate = $"{date:yyyy-MM-dd}";
        PurchasePriceList = new ErpCharacteristicId("Logistics_Procurement_PurchasePriceLists(8fdaa904-47f7-49d3-b5a8-5bbcb02ada4f)");
        CurrencyDirectory = new ErpCharacteristicId("General_CurrencyDirectories(cd9c56b1-2f9b-4ad2-888d-becf3c770cb6)");
        Store = new ErpCharacteristicId("Logistics_Inventory_Stores(100447ff-44f4-4799-a4c2-7c9b22fb0aaa)");
        Supplier = new ErpCharacteristicId("Logistics_Procurement_Suppliers(71887ab9-e1ec-4210-8927-aab5030c3d3b)");
        
        Lines = new List<ErpOrderLinesAccounting>();

    }

    public string DocumentDate { get; set; }
    public string DocumentNo { get; set; }
    public string InvoiceDocumentNo { get; set; }
    public ErpCharacteristicId DocumentType { get; set; }
    public ErpCharacteristicId CurrencyDirectory { get; set; }
    public ErpCharacteristicId FromParty { get; set; }
    public ErpCharacteristicId ToParty { get; set; }
    public ErpCharacteristicId PurchasePriceList { get; set; }
    public ErpCharacteristicId Supplier { get; set; }
    public ErpCharacteristicId PaymentType { get; set; }
    public ErpCharacteristicId Store { get; set; }

    public List<ErpOrderLinesAccounting> Lines { get; set; }
    
}