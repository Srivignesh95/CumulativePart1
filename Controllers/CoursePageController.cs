using Microsoft.AspNetCore.Mvc;
using test1.Models;

namespace test1.Controllers
{
    public class CoursePageController : Controller
    {
        // currently relying on the API to retrieve author information
        // this is a simplified example. In practice, both the AuthorAPI and AuthorPage controllers
        // should rely on a unified "Service", with an explicit interface
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
