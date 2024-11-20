using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using test1.Models;

namespace test1.Controllers
{
    [Route("api/Teacher")]
    [ApiController]
    public class TeacherAPIController : ControllerBase
    {
        private readonly SchoolDB _context;
        // dependency injection of database context
        public TeacherAPIController(SchoolDB context)
        {
            _context = context;
        }


        /// <summary>
        /// Returns a list of Teachers in the system.
        /// </summary>
        /// <example>
        /// GET api/Teacher/ListTeacher -> [{"Teacherid":1,"Teacherfname":"Brian", "Teacherlname":"Smith"},{"Teacherid":2,"Teacherfname":"Jillian", "Teacherlname":"Montgomery"},..]
        /// </example>
        /// <returns>
        /// A list of teacher objects.
        /// </returns>
        [HttpGet]
        [Route(template: "ListTeacher")]
        public List<Teacher> ListTeacher()
        {
            // Create an empty list of Teacher
            List<Teacher> Teacher_list = new List<Teacher>();

            // 'using' will automatically close the connection after the code block
            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();
                // Query to select all teachers
                MySqlCommand Command = Connection.CreateCommand();
                Command.CommandText = "select * from teachers";

                // Execute query and process result
                using (MySqlDataReader ResultSet = Command.ExecuteReader())
                {
                    //Loop Through Each Row the Result Set
                    while (ResultSet.Read())
                    {
                        Teacher CurrentTeacher = new Teacher();
                        //Access Column information by the DB column name as an index
                        CurrentTeacher.Teacherid = Convert.ToInt32(ResultSet["teacherid"]);
                        CurrentTeacher.Teacherfname = (ResultSet["teacherfname"].ToString());
                        CurrentTeacher.Teacherlname = (ResultSet["teacherlname"].ToString());
                        CurrentTeacher.Employeenumber = (ResultSet["employeenumber"].ToString());
                        CurrentTeacher.Hiredate = (ResultSet["hiredate"].ToString());
                        CurrentTeacher.Salary = Convert.ToDecimal(ResultSet["salary"]);
                        Teacher_list.Add(CurrentTeacher);

                    }
                }
            }


            //Return the final list of List
            return Teacher_list;
        }
        /// <summary>
        /// Returns a teacher and their assigned courses by teacher ID.
        /// </summary>
        /// <example>
        /// GET api/Teacher/FindTeacher/3 -> {"Teacherid":3,"Teacherfname":"Sam","Teacherlname":"Cooper", "Coursenames":["Math", "Science"]}
        /// </example>
        /// <returns>
        /// A teacher object with a list of course names.
        /// </returns>
        [HttpGet]
        [Route(template: "FindTeacher/{id}")]
        public Teacher FindTeacher(int id)
        {

            //Empty Teacher
            Teacher CurrentTeacher = new Teacher();

            // 'using' will close the connection after the code executes
            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();
                //Establish a new command (query) for our database
                MySqlCommand Command = Connection.CreateCommand();

                // @id is replaced with a 'sanitized' id
                Command.CommandText = "SELECT t.teacherid, t.teacherfname, t.teacherlname, t.employeenumber, t.hiredate, t.salary, c.courseid, c.coursecode, CASE WHEN c.coursename IS NULL THEN 'No courses assigned' ELSE c.coursename END AS coursename, c.startdate, c.finishdate FROM teachers t LEFT JOIN courses c ON t.teacherid = c.teacherid WHERE t.teacherid = @id";

                Command.Parameters.AddWithValue("@id", id);

                // Gather Result Set of Query into a variable
                using (MySqlDataReader ResultSet = Command.ExecuteReader())
                {
                    //Loop Through Each Row the Result Set
                    while (ResultSet.Read())
                    {
                        CurrentTeacher.Teacherid = Convert.ToInt32(ResultSet["teacherid"]);
                        CurrentTeacher.Teacherfname = (ResultSet["teacherfname"].ToString());
                        CurrentTeacher.Teacherlname = (ResultSet["teacherlname"].ToString());
                        CurrentTeacher.Employeenumber = (ResultSet["employeenumber"].ToString());
                        CurrentTeacher.Hiredate = (ResultSet["hiredate"].ToString());
                        CurrentTeacher.Salary = Convert.ToDecimal(ResultSet["salary"]);
                        //CurrentTeacher.Coursename.Add(ResultSet["coursename"].ToString());
                        Course CurrentCourse = new Course
                    {
                        coursename = ResultSet["coursename"].ToString(),
                    };

                    // Add the course to the teacher's course list
                    CurrentTeacher.Coursename.Add(CurrentCourse);

                    }
                    
                }
            }


            //Return the final list of author names
            return CurrentTeacher;
        }
    }
}
