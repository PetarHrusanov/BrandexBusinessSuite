namespace BrandexBusinessSuite.Models.ErpDocuments;

public class ErpOnlineSale :ErpDocumentSale
{
    public ErpOnlineSale(
        string documentType,
        string enterpriseCompany,
        string enterpriseCompanyLocation,
        string customerId,
        string documentCurrency,
        string paymentAccount,
        string paymentType,
        string fiscalPrintPostDevice,
        string posOperator,
        string posTerminal,
        string posLocation,
        string salesPerson,
        string shipToCustomer,
        string shipToPartyContactMechanism,
        string store,
        string documentNotes,
        string dateString
        )
    {
        DocumentType = new ErpCharacteristicId(documentType);
        EnterpriseCompany = new ErpCharacteristicId(enterpriseCompany);
        EnterpriseCompanyLocation = new ErpCharacteristicId(enterpriseCompanyLocation);
        Customer = new ErpCharacteristicId(customerId);
        DocumentCurrency = new ErpCharacteristicId(documentCurrency);
        PaymentAccount = new ErpCharacteristicId(paymentAccount);
        PaymentType = new ErpCharacteristicId(paymentType);
        FiscalPrinterPosDevice = new ErpCharacteristicId(fiscalPrintPostDevice);
        PosOperator = new ErpCharacteristicId(posOperator);
        PosTerminal = new ErpCharacteristicId(posTerminal);
        PosLocation = new ErpCharacteristicId(posLocation);
        SalesPerson = new ErpCharacteristicId(salesPerson);
        ShipToCustomer = new ErpCharacteristicId(shipToCustomer);
        ShipToPartyContactMechanism = new ErpCharacteristicId(shipToPartyContactMechanism);
        Store = new ErpCharacteristicId(store);
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