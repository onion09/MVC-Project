using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Diagnostics;
using w4d13.Data;
using w4d13.Models;
namespace w4d13.Controllers
{
    //[Route("api/[controller]")]
    //[ApiController]
    public class CrudController: Controller
    {
        private readonly ILogger<CrudController> _logger;
        private readonly CrudModel CrudModel;
        public CrudController(ILogger<CrudController> logger, CrudModel crudModel)
        {
            _logger = logger;
            CrudModel = crudModel;
        }
        public IActionResult Index()
        {

            return View();
        }

        [HttpGet("[action]")]

        public IActionResult GetAllStudents()
        {

            DataTable dt = CrudModel.ExecuteSqlCommand("select * from student");
            return View("Students", dt);
        }
        public IActionResult GetAllProfessors()
        {

            DataTable dt = CrudModel.ExecuteSqlCommand("select * from professor");
            return View("Professors", dt);
        }
        public IActionResult GetStudentToCourse()
        {

            DataTable dt = CrudModel.ExecuteSqlCommand("select * from student_course");
            return View("StudentToCourse", dt);
        }
        public IActionResult GetAllCourses()
        {

            DataTable dt = CrudModel.ExecuteSqlCommand("select * from course");
            return View("Courses", dt);
        }

        [HttpPost("[action]")]
        public IActionResult AddStudent(Student student)
        {
            CrudModel.UpsertWithSelectCmd("select * from student", student);
            return  Ok();
        }

        [HttpPut("[action]")]
        public IActionResult UpdateStudent(Student student)
        {
            CrudModel.UpsertWithSelectCmd("select * from student", student);
            return Ok();
        }

        [HttpPost("[action]")]
        public IActionResult AddCourse(Course course)
        {
            CrudModel.InsertWithSelectCmd("select * from course", course);
            return Ok();
        }
        [HttpPost("[action]")]
        public IActionResult AddProfessor(Professor prof)
        {
            CrudModel.InsertWithSelectCmd("select * from professor", prof);
            return Ok();
        }

        [HttpPost("[action]")]
        public IActionResult AssignStudentToCourse(int studentId, int courseId)
        {
            CrudModel.AssignStudentToCourse( studentId, courseId);
            return Ok();
        }

        [HttpPut("[action]")]
        public IActionResult AssignProfessorToCourse(int profId, int courseId)
        {
            CrudModel.AssignProfessorToCourse(profId, courseId);
            return Ok();
        }

        [HttpGet("[action]")]
        public IActionResult FindStudentCourse(string email)
        {
            List<Course> courses = CrudModel.FindStudentCourse(email);
            return View("Index1", courses);

        }



    }
}
