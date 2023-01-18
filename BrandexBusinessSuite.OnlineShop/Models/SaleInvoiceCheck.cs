using WooCommerceNET.WooCommerce.v3;

namespace BrandexBusinessSuite.OnlineShop.Models;

public class SaleInvoiceCheck
{
    public SaleInvoiceCheck(Order order, double deliveryPrice, string trackingCode)
    {
        Date = $"{order.date_created:yyyy-MM-dd}";
        OrderTotal = (double)order.total!;
        ClientName = order.shipping.first_name+" "+order.shipping.last_name;
        Order = order.id.ToString()!;
        City = order.shipping.city;
        DeliveryPrice = deliveryPrice;
        TrackingCode = trackingCode;
    }

    public string Date { get; set; }
    public double OrderTotal { get; set; }
    public string ClientName { get; set; }
    public string Order { get; set; }
    public string City { get; set; }
    public double DeliveryPrice { get; set; }
    public string TrackingCode { get; set; }
    public string InvoiceNumber { get; set; }
    
    public string Notes { get; set; }
    
}