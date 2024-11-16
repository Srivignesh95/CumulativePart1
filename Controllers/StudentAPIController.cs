using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using test1.Models;

namespace test1.Controllers
{
    [Route("api/Students")]
    [ApiController]
    public class StudentAPIController : ControllerBase
    {
        private readonly SchoolDB _context;
        // dependency injection of database context
        public StudentAPIController(SchoolDB context)
        {
            _context = context;
        }


        /// <summary>
        /// Returns a list of students in the system.
        /// </summary>
        /// <example>
        /// GET api/Students/ListStudents -> [{"Studentid":1,"Studentfname":"Brian", "Studentlname":"Smith"},{"Studentid":2,"Studentfname":"Jillian", "Studentlname":"Montgomery"},..]
        /// </example>
        /// <returns>
        /// A list of student objects.
        /// </returns>
        [HttpGet]
        [Route(template: "ListStudents")]
        public List<Students> ListStudents()
        {
            // Create an empty list of Students
            List<Students> Students_list = new List<Students>();

            // 'using' will close the connection after the code executes
            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();
                //Establish a new command (query) for our database
                MySqlCommand Command = Connection.CreateCommand();

                //SQL QUERY
                Command.CommandText = "select * from students";

                // Gather Result Set of Query into a variable
                using (MySqlDataReader ResultSet = Command.ExecuteReader())
                {
                    //Loop Through Each Row the Result Set
                    while (ResultSet.Read())
                    {
                        Students CurrentStudents = new Students();
                        //Access Column information by the DB column name as an index
                        CurrentStudents.Studentid = Convert.ToInt32(ResultSet["studentid"]);
                        CurrentStudents.Studentfname = (ResultSet["studentfname"].ToString());
                        CurrentStudents.Studentlname = (ResultSet["studentlname"].ToString());
                        CurrentStudents.Studentnumber = (ResultSet["studentnumber"].ToString());
                        CurrentStudents.Enroledate = (ResultSet["enroldate"].ToString());

                        // Add the current student to the list
                        Students_list.Add(CurrentStudents);

                    }
                }
            }


            // Return the final list of students
            return Students_list;
        }
        /// <summary>
        /// Returns a student in the database by their ID along with enrolled courses.
        /// </summary>
        /// <example>
        /// GET api/Students/FindStudent/3 -> {"Studentid":3,"Studentfname":"Sam","Studentlname":"Cooper", "Coursesname":["Math", "Science"]}
        /// </example>
        /// <returns>
        /// A matching student object by its ID. Returns a student object with "No program enrolled" if no courses are associated.
        /// </returns>
        [HttpGet]
        [Route(template: "FindStudent/{id}")]
        public Students FindStudent(int id)
        {

            // Create an empty student object
            Students CurrentStudents = new Students();

            // 'using' will close the connection after the code executes
            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();
                //Establish a new command (query) for our database
                MySqlCommand Command = Connection.CreateCommand();

                // @id is replaced with a 'sanitized' id
                Command.CommandText = "SELECT s.*, CASE WHEN c.coursename IS NULL THEN 'No program enrolled' ELSE c.coursename END AS coursename FROM students s LEFT JOIN studentsxcourses sc ON s.studentid = sc.studentid LEFT JOIN courses c ON sc.courseid = c.courseid WHERE s.studentid = @id;";
                Command.Parameters.AddWithValue("@id", id);

                // Gather Result Set of Query into a variable
                using (MySqlDataReader ResultSet = Command.ExecuteReader())
                {
                    //Loop Through Each Row the Result Set
                    while (ResultSet.Read())
                    {
                        CurrentStudents.Studentid = Convert.ToInt32(ResultSet["studentid"]);
                        CurrentStudents.Studentfname = (ResultSet["studentfname"].ToString());
                        CurrentStudents.Studentlname = (ResultSet["studentlname"].ToString());
                        CurrentStudents.Studentnumber = (ResultSet["studentnumber"].ToString());
                        CurrentStudents.Enroledate = (ResultSet["enroldate"].ToString());
                        CurrentStudents.Coursesname.Add(ResultSet["coursename"].ToString());

                    }
                }
            }


            // Return the student object with associated courses
            return CurrentStudents;
        }
    }
}
