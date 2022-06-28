namespace BrandexSalesAdapter.Models.ErpDocuments;

public class ErpCharacteristicValueDescriptionBg
{

    public ErpCharacteristicValueDescriptionBg()
    {
        Description = new _Description();
    }

    public ErpCharacteristicValueDescriptionBg(string value, _Description description)
    {
        Value = value;
        Description = description;
    }

    public string Value { get; set; }
    
    public _Description Description { get; set; }

    public class _Description
    {
        public _Description() { }
        public _Description(string bg)
        {
            BG = bg;
        }
        public string BG { get; set; }
    }
    
}