using BrandexBusinessSuite.Data.Models.Common;

namespace BrandexBusinessSuite.Accounting.Data.Models;

public class Currency : IAuditInfo, IDeletableEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public double Value { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedOn { get; set; }
}