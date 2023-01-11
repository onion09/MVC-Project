using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Diagnostics;
using w4d13.Data;
using w4d13.Models;

namespace w4d13.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CrudModel CrudModel;

        public HomeController(ILogger<HomeController> logger, CrudModel crudModel)
        {
            _logger = logger;
            CrudModel = crudModel;
        }

        public IActionResult Index()
        {
            DataTable dt = CrudModel.ExecuteSqlCommand("select * from student");
            return View("Students", dt);
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