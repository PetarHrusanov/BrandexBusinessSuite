namespace BrandexSalesAdapter.Accounting.Models;

using BrandexSalesAdapter.Models.ErpDocuments;
using Newtonsoft.Json;

public class MarketingActivityCm : ErpDocument
{
    
    // public _InnerClass InnerClass;
    public MarketingActivityCm()
    {
        // DocumentType = new _DocumentType();
        // EnterpriseCompany = new _EnterpriseCompany();
        // EnterpriseCompanyLocation = new _EnterpriseCompanyLocation();
        ResponsibleParty = new _ResponsibleParty();
        OwnerParty = new _OwnerParty();
        ResponsiblePerson = new _ResponsiblePerson();
        ToParty = new ErpCharacteristicToParty();
        TargetParty = new _TargetParty();
        CustomProperty_МЕСЕЦ = new _CustomProperty_МЕСЕЦ();
        CustomProperty_1579648 = new _CustomProperty_1579648();
        CustomProperty_Размер = new _CustomProperty_Размер();
        CustomProperty_тип_u0020реклама = new _CustomProperty_тип_u0020реклама();
        CustomProperty_ре = new _CustomProperty_ре();
        CustomProperty_novinar = new _CustomProperty_novinar();
        CustomProperty_цена_u0020реклама = new _CustomProperty_цена_u0020реклама();
        CustomProperty_058 = new _CustomProperty_058();
        CustomProperty_ПРОДУКТ_u0020БРАНДЕКС = new _CustomProperty_ПРОДУКТ_u0020БРАНДЕКС();
    }


    // public _DocumentType DocumentType { get; set; }
    // public _EnterpriseCompany EnterpriseCompany { get; set; }
    // public _EnterpriseCompanyLocation EnterpriseCompanyLocation { get; set; }
    public _ResponsibleParty ResponsibleParty { get; set; }
    public _OwnerParty OwnerParty { get; set; }
    public _ResponsiblePerson ResponsiblePerson { get; set; }
    public ErpCharacteristicToParty ToParty { get; set; }
    public _TargetParty TargetParty { get; set; }
    public _CustomProperty_МЕСЕЦ CustomProperty_МЕСЕЦ { get; set; }
    public _CustomProperty_1579648 CustomProperty_1579648 { get; set; }
    public _CustomProperty_Размер CustomProperty_Размер { get; set; }
    public _CustomProperty_тип_u0020реклама CustomProperty_тип_u0020реклама { get; set; }
    public _CustomProperty_ре CustomProperty_ре { get; set; }
    public _CustomProperty_novinar CustomProperty_novinar { get; set; }
    public _CustomProperty_цена_u0020реклама CustomProperty_цена_u0020реклама { get; set; }
    public _CustomProperty_058 CustomProperty_058 { get; set; }
    public _CustomProperty_ПРОДУКТ_u0020БРАНДЕКС CustomProperty_ПРОДУКТ_u0020БРАНДЕКС { get; set; }
    
    
    // public class _DocumentType
    // {
    //     [JsonProperty("@odata.id")]
    //     public string Id { get; set; }
    // }
    
    // public class _EnterpriseCompany
    // {
    //     [JsonProperty("@odata.id")]
    //     public string Id { get; set; }
    // }
    
    // public class _EnterpriseCompanyLocation
    // {
    //     [JsonProperty("@odata.id")]
    //     public string Id { get; set; }
    // }
    public class _ResponsibleParty
    {
        [JsonProperty("@odata.id")]
        public string Id { get; set; }
    }
    public class _OwnerParty
    {
        [JsonProperty("@odata.id")]
        public string Id { get; set; }
    }
    public class _ResponsiblePerson
    {
        [JsonProperty("@odata.id")]
        public string Id { get; set; }
    }
    // public class _ToParty
    // {
    //     [JsonProperty("@odata.id")]
    //     public string Id { get; set; }
    // }
    public class _TargetParty
    {
        [JsonProperty("@odata.id")]
        public string Id { get; set; }
    }
    
    public string SystemType { get; set; }
    
    public string Subject { get; set; }
    
    public string ReferenceDate { get; set; }
    
    public string DeadlineTime { get; set; }
    
    public string StartTime { get; set; }
    
    public class _CustomProperty_МЕСЕЦ
    {
        public string Value { get; set; }
        
    }
   
    public class _CustomProperty_1579648
    {
        public string Value { get; set; }
    }
    
    public class _CustomProperty_Размер
    {
        public string Value { get; set; }
    }
    
    public class _CustomProperty_тип_u0020реклама
    {
        public string Value { get; set; }
    }
    
    public class _CustomProperty_ре
    {
        public string Value { get; set; }
    }
    
    public class _CustomProperty_novinar
    {
        public string Value { get; set; }
    }
    
    public class _CustomProperty_цена_u0020реклама
    {
        public string Value { get; set; }
    }
    
    public class _CustomProperty_058
    {
        public string Value { get; set; }
    }
    
    public class _CustomProperty_ПРОДУКТ_u0020БРАНДЕКС
    {
        public string Value { get; set; }
    }
    
}