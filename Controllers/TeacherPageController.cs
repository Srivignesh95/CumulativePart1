using Microsoft.AspNetCore.Mvc;
using test1.Models;

namespace test1.Controllers
{
    public class TeacherPageController : Controller
    {
        // Currently relying on the API to retrieve teacher information.
        // This is a simplified example. In practice, both the TeacherAPI and TeacherPage controllers
        // should rely on a unified "Service" layer, with an explicit interface for better modularity and maintainability.

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
