using Microsoft.AspNetCore.Mvc;
using test1.Models;

namespace test1.Controllers
{
    public class TeacherPageController : Controller
    {
        // currently relying on the API to retrieve author information
        // this is a simplified example. In practice, both the AuthorAPI and AuthorPage controllers
        // should rely on a unified "Service", with an explicit interface
        private readonly TeacherAPIController _api;

        public TeacherPageController(TeacherAPIController api)
        {
            _api = api;
        }
        public IActionResult List()
        {
            List<Teacher> Teacher = _api.ListTeacher();
            return View(Teacher);
        }
        public IActionResult Show(int id)
        {
            Teacher SelectedTeacher = _api.FindTeacher(id);
            return View(SelectedTeacher);
        }
    }
}
