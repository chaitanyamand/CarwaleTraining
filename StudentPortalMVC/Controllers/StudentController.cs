using Microsoft.AspNetCore.Mvc;
using StudentPortal.Models;
using StudentPortal.ViewModels;
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

    public IActionResult Create()
{
    var vm = new StudentFormViewModel();
    return View(vm);
}


    [HttpPost]
    public IActionResult Create(StudentFormViewModel vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        var student = new Student { Name = vm.Name, Course = vm.Course };
        _repo.Add(student);
        return RedirectToAction("Index");
    }
}
