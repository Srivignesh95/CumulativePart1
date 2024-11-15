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
        /// Returns a list of Authors in the system
        /// </summary>
        /// <example>
        /// GET api/Author/ListAuthors -> [{"AuthorId":1,"AuthorFname":"Brian", "AuthorLName":"Smith"},{"AuthorId":2,"AuthorFname":"Jillian", "AuthorLName":"Montgomery"},..]
        /// </example>
        /// <returns>
        /// A list of author objects 
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



                        //short form for setting all properties while creating the object
                        //Teacher CurrentTeacher = new Teacher();

                        Students_list.Add(CurrentStudents);

                    }
                }
            }


            //Return the final list of authors
            return Students_list;
        }
        /// <summary>
        /// Returns an author in the database by their ID
        /// </summary>
        /// <example>
        /// GET api/Author/FindAuthor/3 -> {"AuthorId":3,"AuthorFname":"Sam","AuthorLName":"Cooper"}
        /// </example>
        /// <returns>
        /// A matching author object by its ID. Empty object if Author not found
        /// </returns>
        [HttpGet]
        [Route(template: "FindStudent/{id}")]
        public Students FindStudent(int id)
        {

            //Empty Author
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


            //Return the final list of author names
            return CurrentStudents;
        }
    }
}
