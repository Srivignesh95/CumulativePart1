using Microsoft.AspNetCore.Mvc;
using test1.Models;

namespace test1.Controllers
{
    public class StudentPageController : Controller
    {
        // Currently relying on the API to retrieve student information.
        // This is a simplified example. In a production environment, both the StudentAPI and 
        // StudentPage controllers should rely on a unified "Service" layer with an explicit interface 
        // to handle business logic and data retrieval.
        private readonly StudentAPIController _api;
        // Constructor for dependency injection of the StudentAPIController.
        public StudentPageController(StudentAPIController api)
        {
            _api = api;
        }
        // Action to list all students using the API.
        public IActionResult List()
        {
            List<Students> Students = _api.ListStudents();
            return View(Students);
        }
        // Action to display details of a selected student by their ID using the API.
        public IActionResult Show(int id)
        {
            Students SelectedStudent = _api.FindStudent(id);
            return View(SelectedStudent);
        }
        // GET : AuthorPage/New
        [HttpGet]
        public IActionResult New(int id)
        {
            return View();
        }

        // POST: AuthorPage/Create
        [HttpPost]
        public IActionResult Create(Students NewStudent)
        {
            int StudentId = _api.AddStudent(NewStudent);

            // redirects to "Show" action on "Author" cotroller with id parameter supplied
            return RedirectToAction("Show", new { id = StudentId });
        }
        // GET : AuthorPage/DeleteConfirm/{id}
        [HttpGet]
        public IActionResult DeleteConfirm(int id)
        {
            Students SelectedStudent = _api.FindStudent(id);
            return View(SelectedStudent);
        }

        // POST: AuthorPage/Delete/{id}
        [HttpPost]
        public IActionResult Delete(int id)
        {
            int StudentId = _api.DeleteStudent(id);
            // redirects to list action
            if (StudentId == 0)
            {
                TempData["Error"] = "The Student you tried to delete does not exist.";
                return RedirectToAction("DeleteConfirm", new { id = id });
            }
            return RedirectToAction("List");
        }
    }
}
