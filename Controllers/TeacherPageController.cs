using Microsoft.AspNetCore.Mvc;
using MySqlX.XDevAPI.Common;
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
        // GET : AuthorPage/New
        [HttpGet]
        public IActionResult New(int id)
        {
            return View();
        }

        // POST: AuthorPage/Create
        [HttpPost]
        public IActionResult Create(Teacher NewTeacher)
        {
            int TeacherId = _api.AddTeacher(NewTeacher);

            // redirects to "Show" action on "Author" cotroller with id parameter supplied
            return RedirectToAction("Show", new { id = TeacherId });
        }
        // GET : AuthorPage/DeleteConfirm/{id}
        [HttpGet]
        public IActionResult DeleteConfirm(int id)
        {
            Teacher SelectedTeacher = _api.FindTeacher(id);
            return View(SelectedTeacher);
        }

        // POST: AuthorPage/Delete/{id}
        [HttpPost]
        public IActionResult Delete(int id)
        {
            int AuthorId = _api.DeleteTeacher(id);
            // redirects to list action
            if (AuthorId == 0)
            {
                TempData["Error"] = "The teacher you tried to delete does not exist.";
                return RedirectToAction("DeleteConfirm", new { id = id });
            }
            return RedirectToAction("List");
        }
        // GET : AuthorPage/Edit/{id}
        [HttpGet]
        public IActionResult Edit(int id)
        {
            Teacher SelectedTeacher = _api.FindTeacher(id);
            return View(SelectedTeacher);
        }

        // POST: AuthorPage/Update/{id}
        [HttpPost]
        public IActionResult Update(int id, string Teacherfname, string Teacherlname, string Employeenumber, string Hiredate, decimal Salary)
        {
            Teacher UpdatedTeacher = new Teacher();
            UpdatedTeacher.Teacherfname = Teacherfname;
            UpdatedTeacher.Teacherlname = Teacherlname;
            UpdatedTeacher.Employeenumber = Employeenumber;
            UpdatedTeacher.Hiredate = Hiredate;
            UpdatedTeacher.Salary = Salary;

            // not doing anything with the response
            _api.UpdateTeacher(id, UpdatedTeacher);
            // redirects to show author
            return RedirectToAction("Show", new { id = id });
        }
    }
}
