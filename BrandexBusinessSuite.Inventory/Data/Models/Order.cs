using BrandexBusinessSuite.Data.Models.Common;

namespace BrandexBusinessSuite.Inventory.Data.Models;

public class Order : IAuditInfo, IDeletableEntity
{

    public Order()
    {
        
    }
    
    public Order(int materialId, int supplierId, double quantity, double price, string notes, DateTime orderDate, DateTime? deliveryDate)
    {
        MaterialId = materialId;
        SupplierId = supplierId;
        Quantity = quantity;
        Price = price;
        Notes = notes;
        OrderDate = orderDate;
        DeliveryDate = deliveryDate;
    }

    public int Id { get; set; }
    public double Quantity { get; set; }
    public double Price { get; set; }
    public string Notes { get; set; }
    public int MaterialId { get; set; }
    public virtual Material Material { get; set; }
    public int SupplierId { get; set; }
    public virtual Supplier Supplier { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime? DeliveryDate { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedOn { get; set; }
}