namespace BrandexBusinessSuite.Inventory.Models.Orders;

using AutoMapper;
using BrandexBusinessSuite.Inventory.Data.Models;
using BrandexBusinessSuite.Models;

public class OrderEditModel  : IMapFrom<Order>
{ 
    public int Id { get; set; }
    public double Quantity { get; set; }
    public double Price { get; set; }
    public string Notes { get; set; }
    public int MaterialId { get; set; }
    public int SupplierId { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime? DeliveryDate { get; set; }

    public virtual void Mapping(Profile mapper)
        => mapper
            .CreateMap<Order, OrderEditModel>();
}