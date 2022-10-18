namespace BrandexBusinessSuite.OnlineShop.Data.Models;

using BrandexBusinessSuite.Data.Models.Common;

public class DeliveryPrice : IAuditInfo, IDeletableEntity
{
    public int Id { get; set; }
    public string ErpId { get; set; }
    public string ErpPriceId { get; set; }
    public decimal Price { get; set; }
    
    public DateTime CreatedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedOn { get; set; }
}