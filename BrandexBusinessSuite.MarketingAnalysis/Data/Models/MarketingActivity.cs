using System.ComponentModel.DataAnnotations.Schema;

namespace BrandexBusinessSuite.MarketingAnalysis.Data.Models;

using BrandexBusinessSuite.Data.Models.Common;
    
public class MarketingActivity :IAuditInfo, IDeletableEntity
{
        
    public int Id { get; set; }
        
    public string Description { get; set; }
    
    public string Notes { get; set; }
    
    public DateTime Date { get; set; }
    
    [Column(TypeName = "decimal(18,4)")]
    public decimal Price { get; set; }
        
    public int ProductId { get; set; }
    public virtual Product Product { get; set; }
    
    public bool Paid { get; set; }
    public bool ErpPublished { get; set; }

    public int AdMediaId { get; set; }
    public virtual AdMedia AdMedia { get; set; }
    
    public int MediaTypeId { get; set; }
    public virtual MediaType MediaType { get; set; }

    public DateTime CreatedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedOn { get; set; }
}