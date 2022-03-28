namespace BrandexSalesAdapter.ExcelLogic.Controllers
{
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
    
    using Data;
    using Data.Models;
    using Models.Distributor;
    using Models;
    
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    
    using static Common.InputOutputConstants.SingleStringConstants;
    using Newtonsoft.Json;

    public class DistributorsController :Controller
    {

        private readonly SpravkiDbContext _context;

        public DistributorsController(SpravkiDbContext context)

        {
            _context = context;
        }

        [HttpGet]
        public async Task<DistributorOutputModel[]> GetDistributors()
        {
            return await _context.Distributors.Select(n => new DistributorOutputModel
            {
                Name = n.Name,
                Id = n.Id
            }).ToArrayAsync();
          
        }
        
        [HttpPost]
        public async Task<string> Import([FromBody]SingleStringInputModel singleStringInputModel)
        {

            var distributor = new Distributor
            {
                Name = singleStringInputModel.SingleStringValue
            };

            await _context.Distributors.AddAsync(distributor);
            await _context.SaveChangesAsync();

            var outputSerialized = JsonConvert.SerializeObject(singleStringInputModel);

            outputSerialized = outputSerialized.Replace(SingleStringValueCapital, SingleStringValueLower);

            return outputSerialized;
        }
    }
}
