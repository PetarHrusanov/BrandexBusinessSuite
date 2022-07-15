namespace BrandexBusinessSuite.Accounting.Models;

using BrandexBusinessSuite.Models.ErpDocuments;

public class MarketingActivityCm : ErpDocument
{
    
    public MarketingActivityCm()
    {
        DocumentType = new ErpCharacteristicId();
        EnterpriseCompany = new ErpCharacteristicId();
        EnterpriseCompanyLocation = new ErpCharacteristicId();
        ResponsibleParty = new ErpCharacteristicId();
        OwnerParty = new ErpCharacteristicId();
        ResponsiblePerson = new ErpCharacteristicId();
        ToParty = new ErpCharacteristicId();
        TargetParty = new ErpCharacteristicId();
        CustomProperty_МЕСЕЦ = new ErpCharacteristicValue();
        CustomProperty_1579648 = new ErpCharacteristicValue();
        CustomProperty_Размер = new ErpCharacteristicValue();
        CustomProperty_тип_u0020реклама = new ErpCharacteristicValue();
        CustomProperty_ре = new ErpCharacteristicValue();
        CustomProperty_novinar = new ErpCharacteristicValue();
        CustomProperty_цена_u0020реклама = new ErpCharacteristicValue();
        CustomProperty_058 = new ErpCharacteristicValue();
        CustomProperty_ПРОДУКТ_u0020БРАНДЕКС = new ErpCharacteristicValue();
    }
    
    public ErpCharacteristicId DocumentType { get; set; }
    public ErpCharacteristicId EnterpriseCompany { get; set; }
    public ErpCharacteristicId EnterpriseCompanyLocation { get; set; }
    
    public string SystemType { get; set; }
    public string Subject { get; set; }
    public ErpCharacteristicId ResponsibleParty { get; set; }
    public string ReferenceDate { get; set; }
    public string DeadlineTime { get; set; }
    public string StartTime { get; set; }
    public ErpCharacteristicId OwnerParty { get; set; }
    public ErpCharacteristicId ResponsiblePerson { get; set; }
    public ErpCharacteristicId ToParty { get; set; }
    public ErpCharacteristicId TargetParty { get; set; }
    public ErpCharacteristicValue CustomProperty_МЕСЕЦ { get; set; }
    public ErpCharacteristicValue CustomProperty_1579648 { get; set; }
    public ErpCharacteristicValue CustomProperty_Размер { get; set; }
    public ErpCharacteristicValue CustomProperty_тип_u0020реклама { get; set; }
    public ErpCharacteristicValue CustomProperty_ре { get; set; }
    public ErpCharacteristicValue CustomProperty_novinar { get; set; }
    public ErpCharacteristicValue CustomProperty_цена_u0020реклама { get; set; }
    public ErpCharacteristicValue CustomProperty_058 { get; set; }
    public ErpCharacteristicValue CustomProperty_ПРОДУКТ_u0020БРАНДЕКС { get; set; }

}