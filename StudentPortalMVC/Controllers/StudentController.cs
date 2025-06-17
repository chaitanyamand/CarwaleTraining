using Microsoft.AspNetCore.Mvc;
using StudentPortal.Models;

namespace StudentPortal.Controllers
{
    public class StudentController : Controller
    {
        private readonly IStudentRepository _repo;

        public StudentController(IStudentRepository repo)
        {
            _repo = repo;
        }

        public IActionResult Index()
        {
            ViewData["Title"] = "List of Students";
            //ViewBag.Message = "This is ViewBag data.";
            //TempData["Note"] = "This is TempData, used across requests.";
            var students = _repo.GetAll();
            return View(students);
        }

        public IActionResult Details(int id)
        {
            var student = _repo.GetById(id);
            if (student == null)
                return NotFound();

            return View(student);
        }
    }
}
