namespace BrandexBusinessSuite.OnlineShop.Models;

public class SaleInvoiceCheck
{
    public SaleInvoiceCheck(string date, double orderTotal, string clientName, string order, string city, double deliveryPrice, string trackingCode)
    {
        Date = date;
        OrderTotal = orderTotal;
        ClientName = clientName;
        Order = order;
        City = city;
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
    
}