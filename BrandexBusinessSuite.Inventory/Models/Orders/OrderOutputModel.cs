namespace BrandexBusinessSuite.Inventory.Models.Orders;

using AutoMapper;
using BrandexBusinessSuite.Inventory.Data.Models;
using BrandexBusinessSuite.Models;

public class OrderOutputModel : IMapFrom<Order>
{
    public int Id { get; set; }
    public double Quantity { get; set; }
    public double Price { get; set; }
    public double PriceQuantity { get; set; }

    public string Notes { get; set; }
    public string MaterialName { get; set; }
    public string SupplierName { get; set; }
    public string OrderDate { get; set; }
    public string? DeliveryDate { get; set; }

    public virtual void Mapping(Profile mapper) 
        => mapper
            .CreateMap<Order, OrderOutputModel>()
            .ForMember(o => o.MaterialName, cfg => cfg
                .MapFrom(o => o.Material.Name))
            .ForMember(o => o.SupplierName, cfg => cfg
                .MapFrom(o => o.Supplier.Name))
            .ForMember(o => o.PriceQuantity, cfg => cfg
                .MapFrom(o => o.Price/o.Quantity))
            .ForMember(o => o.OrderDate, cfg => cfg
                .MapFrom(o => o.OrderDate.ToString("yyyy-MM-dd")))
            .ForMember(o => o.DeliveryDate, cfg => cfg
                .MapFrom(o => o.DeliveryDate != null ? o.DeliveryDate.Value.ToString("yyyy-MM-dd") : null));

}