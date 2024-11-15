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
        /// Returns a list of Authors in the system
        /// </summary>
        /// <example>
        /// GET api/Author/ListAuthors -> [{"AuthorId":1,"AuthorFname":"Brian", "AuthorLName":"Smith"},{"AuthorId":2,"AuthorFname":"Jillian", "AuthorLName":"Montgomery"},..]
        /// </example>
        /// <returns>
        /// A list of author objects 
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




                        //short form for setting all properties while creating the object
                        //Teacher CurrentTeacher = new Teacher();

                        Course_list.Add(CurrentCourse);

                    }
                }
            }


            //Return the final list of authors
            return Course_list;
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
        [Route(template: "FindCourse/{id}")]
        public Course FindCourse(int id)
        {

            //Empty Author
            Course CurrentCourse = new Course();

            // 'using' will close the connection after the code executes
            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();
                //Establish a new command (query) for our database
                MySqlCommand Command = Connection.CreateCommand();

                // @id is replaced with a 'sanitized' id
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
