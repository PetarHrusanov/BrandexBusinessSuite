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

        private readonly SpravkiDbContext _context;

        public DistributorsController(SpravkiDbContext context)

        {

            this._context = context;

        }

        //[Authorize]
        public IActionResult Index()
        {
            var distributors = _context.Distributors.Select(n => n.Name).ToList();

            var distributorsView = new DistributorsCollectionModel
            {
                Distributors = distributors
            };

            return View(distributorsView);
        }

        [HttpGet]
        public async Task<DistributorOutputModel[]> GetDistributors()
        {
            return await this._context.Distributors.Select(n => new DistributorOutputModel
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

            await this._context.Distributors.AddAsync(distributor);
            await this._context.SaveChangesAsync();

            return this.Redirect("Index");
        }
    }
}
