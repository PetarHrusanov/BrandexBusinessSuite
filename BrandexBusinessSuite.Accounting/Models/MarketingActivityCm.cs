namespace BrandexBusinessSuite.Accounting.Models;

using BrandexBusinessSuite.Models.ErpDocuments;

public class MarketingActivityCm : ErpDocument
{

    public MarketingActivityCm()
    {
        
    }
    public MarketingActivityCm(string subject,
        DateTime date,
        string partyId,
        string monthErp,
        string yearErp,
        string measure,
        string type,
        string media,
        string publishType,
        double price,
        string product
        )
    {
        DocumentType = new ErpCharacteristicId("General_DocumentTypes(59b265f7-391a-4226-8bcb-44e192ba5690)");
        EnterpriseCompany = new ErpCharacteristicId("General_EnterpriseCompanies(2c186d87-e81d-4318-9a7f-3cfb5399c0d0)");
        EnterpriseCompanyLocation = new ErpCharacteristicId("General_Contacts_CompanyLocations(902743f5-6076-4b5e-b725-2daa192c71f6)");
        SystemType = "Task";
        Subject = subject;
        ResponsibleParty = new ErpCharacteristicId("General_Contacts_Parties(2469d153-839f-445a-b7c2-2e7cb955c491)");
        ReferenceDate = $"{date:yyyy-MM-dd}";
        StartTime = $"{date:yyyy-MM-dd}";
        DeadlineTime = $"{date:yyyy-MM-dd}";
        OwnerParty = new ErpCharacteristicId("General_Contacts_Parties(2469d153-839f-445a-b7c2-2e7cb955c491)");
        ResponsiblePerson = new ErpCharacteristicId("General_Contacts_Persons(623ed5c7-2eec-4e5b-a0c1-42c6faab3309)");
        ToParty = new ErpCharacteristicId($"General_Contacts_Parties({partyId})");
        TargetParty = new ErpCharacteristicId($"General_Contacts_Parties({partyId})");
        CustomProperty_МЕСЕЦ = new ErpCharacteristicValue(monthErp);
        CustomProperty_1579648 = new ErpCharacteristicValue(yearErp);
        CustomProperty_Размер = new ErpCharacteristicValue(measure);
        CustomProperty_тип_u0020реклама = new ErpCharacteristicValue(type);
        CustomProperty_ре = new ErpCharacteristicValue(media);
        CustomProperty_novinar = new ErpCharacteristicValue(publishType);
        CustomProperty_цена_u0020реклама = new ErpCharacteristicValue($"{price}");
        CustomProperty_058 = new ErpCharacteristicValue("");
        CustomProperty_ПРОДУКТ_u0020БРАНДЕКС = new ErpCharacteristicValue(product);
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