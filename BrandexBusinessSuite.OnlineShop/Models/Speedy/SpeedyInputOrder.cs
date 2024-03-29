namespace BrandexBusinessSuite.OnlineShop.Models.Speedy;

using Newtonsoft.Json;
using WooCommerceNET.WooCommerce.v3;
public class SpeedyInputOrder
{
    // public SpeedyInputOrder(string userName, string password, _Service service, _Recipient recipient, string ref1)
    // {
    //     UserName = userName;
    //     Password = password;
    //     Service = service;
    //     Content = new _Content();
    //     Payment = new _Payment();
    //     Recipient = recipient;
    //     Ref1 = ref1;
    // }
    
    public SpeedyInputOrder(string userName, string password, double orderPrice, Order order)
    {
        UserName = userName;
        Password = password;
        Service = new _Service(orderPrice);
        Content = new _Content();
        Payment = new _Payment();
        Recipient = new _Recipient(order);
        Ref1 = order.id.ToString();
    }
    
    [JsonProperty("userName")]
    public string UserName { get; set; }
    
    [JsonProperty("password")]
    public string Password { get; set; }
    
    [JsonProperty("service")]
    public _Service Service { get; set; }
    
    [JsonProperty("content")]
    public _Content Content { get; set; }
    
    [JsonProperty("payment")]
    public _Payment Payment { get; set; }
    
    [JsonProperty("recipient")]
    public _Recipient Recipient { get; set; }

    [JsonProperty("shipmentNote")]
    public string ShipmentNote { get; set; } = "";
    
    [JsonProperty("ref1")]
    public string Ref1 { get; set; }
    
    [JsonProperty("ref2")]
    public string Ref2 { get; set; } = "";

    public class _Service
    {

        // public _Service(int serviceCode, double orderPrice)
        // {
        //     Id = serviceCode;
        //     AdditionalServices = new _AdditionalServices(orderPrice);
        // }
        
        public _Service(double orderPrice)
        {
            Id = 505;
            AdditionalServices = new _AdditionalServices(orderPrice);
        }
        
        [JsonProperty("serviceId")]
        public int Id { get; set; }
        
        [JsonProperty("additionalServices")]
        public _AdditionalServices AdditionalServices { get; set; }
        

        public class _AdditionalServices
        {

            public _AdditionalServices(double orderPrice)
            {
                Cod = new _Cod(orderPrice);
            }
            
            [JsonProperty("cod")]
            public _Cod Cod { get; set; }
            
            public class _Cod
            {
                public _Cod(double orderPrice)
                {
                    OrderPrice = orderPrice;
                }
                [JsonProperty("amount")]
                public double OrderPrice { get; set; } 
            }
        }
    }

    public class _Content
    {
        [JsonProperty("parcelsCount")]
        public int ParcelCount { get; set; } = 1;
        [JsonProperty("totalWeight")]
        public double TotalWeight { get; set; } = 0.3;
        [JsonProperty("contents")]
        public string Contents { get; set; } = "ХРАНИТЕЛНА ДОБАВКА";
        [JsonProperty("package")]
        public string Package { get; set; } = "КУТИЯ В ПЛИК";
    }

    public class _Payment
    {
        [JsonProperty("courierServicePayer")]
        public string CourierServicePayer { get; set; } = "SENDER";
    }

    public class _Recipient
    {

        public _Recipient(
            string phoneNumber,
            string clientName,
            string siteName,
            string postCode, 
            string addressNote
            )
        {
            PhoneNumber = new _PhoneNumber(phoneNumber);
            ClientName = clientName;
            Address = new _Address(siteName, postCode, addressNote);
        }

        public _Recipient(Order order)
        {
            PhoneNumber = new _PhoneNumber(order.billing.phone);
            ClientName = order.shipping.first_name+" "+order.shipping.last_name;
            Address = new _Address(order.shipping.city, order.shipping.postcode, order.shipping.address_1);
        }

        [JsonProperty("phone1")]
        public _PhoneNumber PhoneNumber { get; set; }
        
        [JsonProperty("privatePerson")]
        public bool PrivatePerson { get; set; } = true;
        
        [JsonProperty("clientName")]
        public string ClientName { get; set; }
        
        [JsonProperty("address")]
        public _Address Address { get; set; }
        
        
        public class _PhoneNumber
        {
            public _PhoneNumber(string number)
            {
                Number = number;
            }
            [JsonProperty("number")]
            public string Number { get; set; }
        }

        public class _Address
        {
            public _Address(string siteName, string postCode, string addressNote)
            {
                SiteName = siteName;
                PostCode = postCode;
                AddressNote = addressNote;
            }
            [JsonProperty("siteType")]
            public string SiteType { get; set; } = "гр.";
            
            [JsonProperty("siteName")]
            public string SiteName { get; set; }
            
            [JsonProperty("postCode")]
            public string PostCode { get; set; }
            
            [JsonProperty("addressNote")]
            public string AddressNote { get; set; }
            
        }
    }

}