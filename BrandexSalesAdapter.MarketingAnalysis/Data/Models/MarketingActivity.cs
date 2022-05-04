namespace BrandexSalesAdapter.MarketingAnalysis.Data.Models
{
    using BrandexSalesAdapter.Data.Models.Common;
    
    public class MarketingActivity :IAuditInfo, IDeletableEntity
    {
        
        public int Id { get; set; }
        
        public string Description { get; set; }
        
        public DateTime DateTime { get; set; }
        
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
        
        public int AdMediaId { get; set; }
        public virtual AdMedia AdMedia { get; set; }

        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
    }   
}