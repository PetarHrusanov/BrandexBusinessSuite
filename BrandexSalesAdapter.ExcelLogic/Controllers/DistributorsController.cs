namespace BrandexSalesAdapter.ExcelLogic.Controllers
{
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
    using BrandexSalesAdapter.ExcelLogic.Data;
    using BrandexSalesAdapter.ExcelLogic.Data.Models;
    using BrandexSalesAdapter.ExcelLogic.Models.Distributor;
    using Microsoft.AspNetCore.Authorization;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;

    public class DistributorsController :Controller
    {

        private readonly SpravkiDbContext context;

        public DistributorsController(SpravkiDbContext context)

        {

            this.context = context;

        }

        //[Authorize]
        public IActionResult Index()
        {
            var distributors = context.Distributors.Select(n => n.Name).ToList();

            var distributorsView = new DistributorsCollectionModel
            {
                Distributors = distributors
            };

            return View(distributorsView);
        }

        [HttpGet]
        public async Task<DistributorOutputModel[]> GetDistributors()
        {
            return await this.context.Distributors.Select(n => new DistributorOutputModel
            {
                Name = n.Name,
                Id = n.Id
            }).ToArrayAsync();
          
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ImportAsync(string name)
        {

            var distributor = new Distributor();

            distributor.Name = name;

            await this.context.Distributors.AddAsync(distributor);
            await this.context.SaveChangesAsync();

            return this.Redirect("Index");
        }
    }
}
