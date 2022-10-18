using BrandexBusinessSuite.Models.ErpDocuments;

namespace BrandexBusinessSuite.OnlineShop.Models;

public class ErpLot
{
    public string Id { get; set; }
    public string ExpiryDate { get; set; }
    public string ReceiptDate { get; set; }
    
    public ErpCharacteristicId Product { get; set; }
}