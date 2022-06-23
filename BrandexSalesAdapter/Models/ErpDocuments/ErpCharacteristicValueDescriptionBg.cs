namespace BrandexSalesAdapter.Models.ErpDocuments;

public class ErpCharacteristicValueDescriptionBg
{

    public ErpCharacteristicValueDescriptionBg()
    {
        Description = new _Description();
    }
    
    public string Value { get; set; }
    
    public _Description Description { get; set; }

    public class _Description
    {
        public string BG { get; set; }
    }
    
}