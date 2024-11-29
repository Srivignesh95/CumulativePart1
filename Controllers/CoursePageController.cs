using Microsoft.AspNetCore.Mvc;
using test1.Models;

namespace test1.Controllers
{
    public class CoursePageController : Controller
    {
        // Currently relying on the API to retrieve course information.
        // This is a simplified example. In a production environment, both the CourseAPI and 
        // CoursePage controllers should rely on a unified "Service" layer with an explicit interface 
        // to handle business logic and data retrieval.

        private readonly CourseAPIController _api;

        public CoursePageController(CourseAPIController api)
        {
            _api = api;
        }
        public IActionResult List()
        {
            List<Course> Course = _api.ListCourse();
            return View(Course);
        }
        public IActionResult Show(int id)
        {
            Course SelectedCourse = _api.FindCourse(id);
            return View(SelectedCourse);
        }
        // GET : AuthorPage/New
        [HttpGet]
        public IActionResult New(int id)
        {
            return View();
        }

        // POST: AuthorPage/Create
        [HttpPost]
        public IActionResult Create(Course NewCourse)
        {
            int CourseId = _api.AddCourse(NewCourse);

            // redirects to "Show" action on "Author" cotroller with id parameter supplied
            return RedirectToAction("Show", new { id = CourseId });
        }
        // GET : AuthorPage/DeleteConfirm/{id}
        [HttpGet]
        public IActionResult DeleteConfirm(int id)
        {
            Course SelectedCourse = _api.FindCourse(id);
            return View(SelectedCourse);
        }

        // POST: AuthorPage/Delete/{id}
        [HttpPost]
        public IActionResult Delete(int id)
        {
            int CourseId = _api.DeleteCourse(id);
            // redirects to list action
            if (CourseId == 0)
            {
                TempData["Error"] = "The Course you tried to delete does not exist.";
                return RedirectToAction("DeleteConfirm", new { id = id });
            }
            return RedirectToAction("List");
        }
    }
}
