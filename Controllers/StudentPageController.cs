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
    }
}
