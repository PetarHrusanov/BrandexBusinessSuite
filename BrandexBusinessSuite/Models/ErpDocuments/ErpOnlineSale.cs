namespace BrandexBusinessSuite.Models.ErpDocuments;

public class ErpOnlineSale :ErpDocumentSale
{
    public ErpOnlineSale(string documentNotes, string dateString)
    {
        DocumentType = new ErpCharacteristicId("General_DocumentTypes(1f758f75-6b2d-4c0f-a630-09e2aa893348)");
        EnterpriseCompany = new ErpCharacteristicId("General_EnterpriseCompanies(2c186d87-e81d-4318-9a7f-3cfb5399c0d0)");
        EnterpriseCompanyLocation = new ErpCharacteristicId("General_Contacts_CompanyLocations(0396623b-ee4e-4a57-867f-5a433d8b6440)");
        Customer = new ErpCharacteristicId("Crm_Customers(6053262d-3544-4cbb-a322-7036c07570ef)");
        DocumentCurrency = new ErpCharacteristicId("General_Currencies(3187833a-d3c1-4804-bfc0-e17e6aee3069)");
        PaymentAccount = new ErpCharacteristicId("Finance_Payments_PaymentAccounts(708814be-0e15-45e3-854a-e2c0235b3231)");
        PaymentType = new ErpCharacteristicId("Finance_Payments_PaymentTypes(2ae637f8-a1ab-4a47-b3db-3ecbd40b821c)");
        FiscalPrinterPosDevice = new ErpCharacteristicId("Crm_Pos_Devices(99dd9d0e-3a74-4ec2-b16d-3c7be3c95ac0)");
        PosOperator = new ErpCharacteristicId("Crm_Pos_Operators(14adcaae-40b9-40f3-b4dc-94c276804c4c)");
        PosTerminal = new ErpCharacteristicId("Crm_Pos_Terminals(e70d0703-0e2c-4155-890a-a829daf68ef5)");
        PosLocation = new ErpCharacteristicId("Crm_Pos_Locations(263ed143-67f0-4309-86ff-7bb632fb9730)");
        SalesPerson = new ErpCharacteristicId("Crm_SalesPersons(6a5ca246-d73c-431e-ba5c-460abc9f6af6)");
        ShipToCustomer = new ErpCharacteristicId( "Crm_Customers(e4367b97-b2f3-4bd6-822f-59b8efd1a4c4)");
        ShipToPartyContactMechanism = new ErpCharacteristicId("General_Contacts_PartyContactMechanisms(9fad9148-d170-48f9-bcfe-a1ac7ffe44e4)");
        Store = new ErpCharacteristicId( "Logistics_Inventory_Stores(100447ff-44f4-4799-a4c2-7c9b22fb0aaa)");
        DocumentNotes = documentNotes;
        RequiredDeliveryDate = dateString;
        PaymentDueDate = dateString;
        PaymentDueStartDate = dateString;
        Lines = new List<ErpSalesLines>();
    }

    public ErpCharacteristicId Customer { get; set; }
    public ErpCharacteristicId FiscalPrinterPosDevice { get; set; }
    public ErpCharacteristicId PosOperator { get; set; }
    public ErpCharacteristicId PosTerminal { get; set; }
    public ErpCharacteristicId PosLocation { get; set; }
    public ErpCharacteristicId SalesPerson { get; set; }
    public ErpCharacteristicId ShipToCustomer { get; set; }
    public ErpCharacteristicId ShipToPartyContactMechanism { get; set; }
    public ErpCharacteristicId Store { get; set; }
    
    public string DocumentNotes { get; set; }
    public string RequiredDeliveryDate { get; set; }
    public string PaymentDueDate { get; set; }
    public string PaymentDueStartDate { get; set; }
    public string CreditLimitOverride { get; set; } = "True";
    
    public List<ErpSalesLines> Lines { get; set; }
    
}