using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Diagnostics;
using w4d13.Data;
using w4d13.Models;
namespace w4d13.Controllers
{
    public class CrudController: Controller
    {
        private readonly ILogger<CrudController> _logger;
        private readonly CrudModel CrudModel;
        public CrudController(ILogger<CrudController> logger, CrudModel crudModel)
        {
            _logger = logger;
            CrudModel = crudModel;
        }
        public IActionResult GetAllStudents()
        {

            DataTable dt = CrudModel.GetAllStudents();
            return View("Students", dt);
        }
    }
}
