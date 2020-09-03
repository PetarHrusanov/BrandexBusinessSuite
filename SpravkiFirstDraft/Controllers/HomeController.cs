using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using SpravkiFirstDraft.Data;
using SpravkiFirstDraft.Data.Enums;
using SpravkiFirstDraft.Data.Models;
using SpravkiFirstDraft.Models;

namespace SpravkiFirstDraft.Controllers
{
    public class HomeController : Controller
    {
        private IWebHostEnvironment hostEnvironment;

        private readonly SpravkiDbContext context;

        public HomeController(IWebHostEnvironment hostEnvironment, SpravkiDbContext context)

        {

            this.hostEnvironment = hostEnvironment;
            this.context = context;

        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
