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
        [HttpGet]
        public IActionResult SearchByHireDate(DateTime minDate, DateTime maxDate)
        {
            List<Teacher> teachers = _api.FindTeachersByHireDate(minDate, maxDate);
            ViewData["MinDate"] = minDate.ToString("yyyy-MM-dd");
            ViewData["MaxDate"] = maxDate.ToString("yyyy-MM-dd");
            return View(teachers); // Ensure the "SearchByHireDate.cshtml" view is created.
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
            IActionResult response  = _api.AddTeacher(NewTeacher);

            if (response is BadRequestObjectResult badRequest)
            {
                // Extract the error message
                var error = (badRequest.Value as dynamic)?.Message ?? "An error occurred.";
                ViewBag.ErrorMessage = error;
                return View("New", NewTeacher); // Return to the form with the error message
            }

            if (response is OkObjectResult okResult)
            {
                int teacherId = (okResult.Value as dynamic).TeacherId;
                return RedirectToAction("Show", new { id = teacherId });
            }

            // Handle unexpected scenarios
            ViewBag.ErrorMessage = "An unexpected error occurred.";
            return View("New", NewTeacher);
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
            int teacher = Convert.ToInt32(_api.FindTeacher(id)); // Ensure `FindTeacher` correctly queries the database.

            Teacher UpdatedTeacher = new Teacher();
            UpdatedTeacher.Teacherfname = Teacherfname;
            UpdatedTeacher.Teacherlname = Teacherlname;
            UpdatedTeacher.Employeenumber = Employeenumber;
            UpdatedTeacher.Hiredate = Hiredate;
            UpdatedTeacher.Salary = Salary;

            // not doing anything with the response
            IActionResult response = _api.UpdateTeacher(id, UpdatedTeacher);
            // redirects to show author
            if (response is OkObjectResult)
            {
                // Redirect to the details page of the updated teacher
                return RedirectToAction("Show", new { id = id });
            }

            // Handle unexpected scenarios
            TempData["Error"] = "An unexpected error occurred.";
            return RedirectToAction("Edit", new { id = id });
        }
    }
}
