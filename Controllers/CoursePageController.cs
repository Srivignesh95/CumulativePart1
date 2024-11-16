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
    }
}
