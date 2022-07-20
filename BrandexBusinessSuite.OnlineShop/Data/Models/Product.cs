namespace BrandexBusinessSuite.OnlineShop.Data.Models;

using BrandexBusinessSuite.Data.Models.Common;

public class Product : IAuditInfo, IDeletableEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string ErpCode { get; set; }
    public string ErpPriceCode { get; set; }
    public decimal ErpPriceNoVat { get; set; }
    public string ErpLot { get; set; }
    public string WooCommerceName { get; set; }
    
    public string WooCommerceSampleName { get; set; }
    public string ErpSampleCode { get; set; }
    
    public virtual ICollection<SaleOnlineAnalysis> SaleOnlineAnalysis { get; set; }

    public DateTime CreatedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedOn { get; set; }
}