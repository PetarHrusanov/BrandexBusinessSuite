namespace BrandexBusinessSuite.Models.ErpDocuments;

public class ErpInvoice : ErpDocumentSale
{

    public ErpInvoice(string note, string deliveryDate)
    {
        DocumentNotes = note;
        // DocumentType = new ErpCharacteristicId("General_DocumentTypes(db26beb5-3604-4ab3-a8c7-a417d2c4ac59)");
        DocumentType = new ErpCharacteristicId("Systems_Documents_DocumentTypes(db26beb5-3604-4ab3-a8c7-a417d2c4ac59)");
        EnterpriseCompany = new ErpCharacteristicId("General_EnterpriseCompanies(2c186d87-e81d-4318-9a7f-3cfb5399c0d0)");
        EnterpriseCompanyLocation = new ErpCharacteristicId("General_Contacts_CompanyLocations(0396623b-ee4e-4a57-867f-5a433d8b6440)");
        DocumentCurrency = new ErpCharacteristicId("General_Currencies(3187833a-d3c1-4804-bfc0-e17e6aee3069)");
        PaymentAccount = new ErpCharacteristicId("Finance_Payments_PaymentAccounts(708814be-0e15-45e3-854a-e2c0235b3231)");
        PaymentType = new ErpCharacteristicId("Finance_Payments_PaymentTypes(2ae637f8-a1ab-4a47-b3db-3ecbd40b821c)");
        FromParty = new ErpCharacteristicId("General_Contacts_Parties(42bef242-101f-48bd-b6c5-8da6819c844f)");
        ResponsiblePerson = new ErpCharacteristicId("General_Contacts_Persons(eac9ba14-cf12-4f8c-a69c-d095dd347a1e)");
        ToParty = new ErpCharacteristicId("General_Contacts_Parties(5a262172-ae99-4a68-b38d-eef0778137dd)");
        Customer = new ErpCharacteristicId("Crm_Customers(6053262d-3544-4cbb-a322-7036c07570ef)");
        DealType =  new ErpCharacteristicId("Finance_Vat_DealTypes(7e483e21-3b9b-489e-bd80-b3373cc9ed3e)");
        Lines = new List<ErpInvoiceLines>();
        DeliveryDate = deliveryDate;

    }

    public string DocumentNotes { get; set; }
    public ErpCharacteristicProductName PaymentTypeDescription { get; set; } = new ErpCharacteristicProductName("");
    public ErpCharacteristicId FromParty { get; set; }
    public ErpCharacteristicId ResponsiblePerson { get; set; }
    public ErpCharacteristicId ToParty { get; set; }
    public ErpCharacteristicId Customer { get; set; }
    public ErpCharacteristicId DealType { get; set; }
    public List<ErpInvoiceLines> Lines { get; set; }
    
    public string DeliveryDate { get; set; }

}

