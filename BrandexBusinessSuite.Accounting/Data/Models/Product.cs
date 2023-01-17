namespace BrandexBusinessSuite.Accounting.Data.Models;

using BrandexBusinessSuite.Data.Models.Common;

public class Product : IAuditInfo, IDeletableEntity
{
    
    public int Id { get; set; }
    public string Name { get; set; }
    public string FacebookName { get; set; }
    public string GoogleName { get; set; }
    public string GoogleNameErp { get; set; }
    public string AccountingName { get; set; }
    public string AccountingErpNumber { get; set; }
    
    public DateTime CreatedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedOn { get; set; }
}