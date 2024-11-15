using Microsoft.AspNetCore.Mvc;
using test1.Models;

namespace test1.Controllers
{
    public class StudentPageController : Controller
    {
        // currently relying on the API to retrieve author information
        // this is a simplified example. In practice, both the AuthorAPI and AuthorPage controllers
        // should rely on a unified "Service", with an explicit interface
        private readonly StudentAPIController _api;

        public StudentPageController(StudentAPIController api)
        {
            _api = api;
        }
        public IActionResult List()
        {
            List<Students> Students = _api.ListStudents();
            return View(Students);
        }
        public IActionResult Show(int id)
        {
            Students SelectedStudent = _api.FindStudent(id);
            return View(SelectedStudent);
        }
    }
}
