namespace BrandexSalesAdapter.Models.ErpDocuments;

using Newtonsoft.Json;

public abstract class ErpDocument
{

    public ErpDocument()
    {
        DocumentType = new ErpCharacteristicId();
        EnterpriseCompany = new ErpCharacteristicId();
        EnterpriseCompanyLocation = new ErpCharacteristicId();
    }
    
    public ErpCharacteristicId DocumentType { get; set; }
    public ErpCharacteristicId EnterpriseCompany { get; set; }
    public ErpCharacteristicId EnterpriseCompanyLocation { get; set; }
    
    
    
}