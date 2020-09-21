namespace BrandexSalesAdapter.ExcelLogic.Controllers
{
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
    using BrandexSalesAdapter.ExcelLogic.Data;
    using BrandexSalesAdapter.ExcelLogic.Data.Models;
    using BrandexSalesAdapter.ExcelLogic.Models.Distributor;

    public class DistributorsController :Controller
    {

        private readonly SpravkiDbContext context;

        public DistributorsController(SpravkiDbContext context)

        {

            this.context = context;

        }

        public IActionResult Index()
        {
            var distributors = context.Distributors.Select(n => n.Name).ToList();

            var distributorsView = new DistributorsCollectionModel
            {
                Distributors = distributors
            };

            return View(distributorsView);
        }

        public async System.Threading.Tasks.Task<IActionResult> ImportAsync(string name)
        {

            var distributor = new Distributor();

            distributor.Name = name;

            await this.context.Distributors.AddAsync(distributor);
            await this.context.SaveChangesAsync();

            return this.Redirect("Index");
        }
    }
}
