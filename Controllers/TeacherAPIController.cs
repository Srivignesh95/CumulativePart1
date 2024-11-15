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
        /// Returns a list of Authors in the system
        /// </summary>
        /// <example>
        /// GET api/Author/ListAuthors -> [{"AuthorId":1,"AuthorFname":"Brian", "AuthorLName":"Smith"},{"AuthorId":2,"AuthorFname":"Jillian", "AuthorLName":"Montgomery"},..]
        /// </example>
        /// <returns>
        /// A list of author objects 
        /// </returns>
        [HttpGet]
        [Route(template: "ListTeacher")]
        public List<Teacher> ListTeacher()
        {
            // Create an empty list of Teacher
            List<Teacher> Teacher_list = new List<Teacher>();

            // 'using' will close the connection after the code executes
            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();
                //Establish a new command (query) for our database
                MySqlCommand Command = Connection.CreateCommand();

                //SQL QUERY
                Command.CommandText = "select * from teachers";

                // Gather Result Set of Query into a variable
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



                        //short form for setting all properties while creating the object
                        //Teacher CurrentTeacher = new Teacher();

                        Teacher_list.Add(CurrentTeacher);

                    }
                }
            }


            //Return the final list of authors
            return Teacher_list;
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
        [Route(template: "FindTeacher/{id}")]
        public Teacher FindTeacher(int id)
        {

            //Empty Author
            Teacher CurrentTeacher = new Teacher();

            // 'using' will close the connection after the code executes
            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();
                //Establish a new command (query) for our database
                MySqlCommand Command = Connection.CreateCommand();

                // @id is replaced with a 'sanitized' id
                Command.CommandText = "SELECT teachers.teacherid, teachers.teacherfname, teachers.teacherlname, teachers.employeenumber, teachers.hiredate, teachers.salary, courses.courseid, courses.coursecode, courses.coursename, courses.startdate, courses.finishdate FROM teachers INNER JOIN courses ON teachers.teacherid = courses.teacherid WHERE teachers.teacherid = @id;";

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
                        CurrentTeacher.Coursename.Add(ResultSet["coursename"].ToString());

                    }
                    
                }
            }


            //Return the final list of author names
            return CurrentTeacher;
        }
    }
}
