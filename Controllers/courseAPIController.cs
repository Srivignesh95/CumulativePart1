using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using test1.Models;

namespace test1.Controllers
{
    [Route("api/Course")]
    [ApiController]
    public class CourseAPIController : ControllerBase
    {
        private readonly SchoolDB _context;
        // dependency injection of database context
        public CourseAPIController(SchoolDB context)
        {
            _context = context;
        }


        /// <summary>
        /// Returns a list of courses in the system along with their associated teacher names.
        /// </summary>
        /// <example>
        /// GET api/Course/ListCourse -> [{"CourseId":1,"CourseCode":"CS101","TeacherId":3,"StartDate":"2023-01-01","FinishDate":"2023-06-01","CourseName":"Introduction to Programming","TeacherName":"John Doe"},...]
        /// </example>
        /// <returns>
        /// A list of course objects including course details and the teacher's full name.
        /// </returns>
        [HttpGet]
        [Route(template: "ListCourse")]
        public List<Course> ListCourse()
        {
            // Create an empty list of Course
            List<Course> Course_list = new List<Course>();

            // 'using' will close the connection after the code executes
            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();
                //Establish a new command (query) for our database
                MySqlCommand Command = Connection.CreateCommand();

                //SQL QUERY
                Command.CommandText = "SELECT c.*, CONCAT( t.teacherfname,' ', t.teacherlname) AS Techername FROM courses c JOIN teachers t ON c.teacherid = t.teacherid;";

                // Gather Result Set of Query into a variable
                using (MySqlDataReader ResultSet = Command.ExecuteReader())
                {
                    //Loop Through Each Row the Result Set
                    while (ResultSet.Read())
                    {
                        Course CurrentCourse = new Course();
                        //Access Column information by the DB column name as an index
                        CurrentCourse.Courseid = Convert.ToInt32(ResultSet["courseid"]);
                        CurrentCourse.Coursecode = (ResultSet["coursecode"].ToString());
                        CurrentCourse.Teacherid = Convert.ToInt32(ResultSet["teacherid"]);
                        CurrentCourse.Startdate = (ResultSet["startdate"].ToString());
                        CurrentCourse.Finishdate = (ResultSet["finishdate"].ToString());
                        CurrentCourse.coursename = (ResultSet["coursename"].ToString());
                        CurrentCourse.Teachername = (ResultSet["Techername"].ToString());
                        // Add the current course to the list
                        Course_list.Add(CurrentCourse);

                    }
                }
            }


            //Return the final list of authors
            return Course_list;
        }
        /// <summary>
        /// Returns a course in the database by its ID along with the associated teacher name.
        /// </summary>
        /// <example>
        /// GET api/Course/FindCourse/1 -> {"CourseId":1,"CourseCode":"CS101","TeacherId":3,"StartDate":"2023-01-01","FinishDate":"2023-06-01","CourseName":"Introduction to Programming","TeacherName":"John Doe"}
        /// </example>
        /// <returns>
        /// A course object matching the given ID. Returns an empty object if the course is not found.
        /// </returns>
        [HttpGet]
        [Route(template: "FindCourse/{id}")]
        public Course FindCourse(int id)
        {

            // Create an empty course object
            Course CurrentCourse = new Course();

            // 'using' ensures the database connection is properly closed after execution
            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();
                // Establish a new command (query) for the database
                MySqlCommand Command = Connection.CreateCommand();

                // SQL query to retrieve course information and teacher name by course ID
                Command.CommandText = "SELECT c.*, CONCAT( t.teacherfname,' ', t.teacherlname) AS Techername FROM courses c JOIN teachers t ON c.teacherid = t.teacherid where courseid=@id;";
                Command.Parameters.AddWithValue("@id", id);

                // Gather Result Set of Query into a variable
                using (MySqlDataReader ResultSet = Command.ExecuteReader())
                {
                    //Loop Through Each Row the Result Set
                    while (ResultSet.Read())
                    {
                        CurrentCourse.Courseid = Convert.ToInt32(ResultSet["courseid"]);
                        CurrentCourse.Coursecode = (ResultSet["coursecode"].ToString());
                        CurrentCourse.Teacherid = Convert.ToInt32(ResultSet["teacherid"]);
                        CurrentCourse.Startdate = (ResultSet["startdate"].ToString());
                        CurrentCourse.Finishdate = (ResultSet["finishdate"].ToString());
                        CurrentCourse.coursename = (ResultSet["coursename"].ToString());
                        CurrentCourse.Teachername = (ResultSet["Techername"].ToString());

                    }
                }
            }


            //Return the final list of author names
            return CurrentCourse;
        }
    }
}
