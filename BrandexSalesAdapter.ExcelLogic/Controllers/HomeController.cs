namespace BrandexSalesAdapter.ExcelLogic.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;

    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _hostEnvironment;

        public HomeController(
            IWebHostEnvironment hostEnvironment)


        {
            _hostEnvironment = hostEnvironment;
        }

        public Task<IActionResult> Index()
        {
            return Task.FromResult<IActionResult>(View());
        }
    }
}