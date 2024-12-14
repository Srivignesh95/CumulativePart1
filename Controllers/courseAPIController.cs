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
                Command.CommandText = "SELECT c.*, CASE WHEN t.teacherid IS NULL THEN 'Teacher''s code is not valid' ELSE CONCAT(t.teacherfname, ' ', t.teacherlname) END AS Techername FROM courses c LEFT JOIN teachers t ON c.teacherid = t.teacherid;";

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
                        //CurrentCourse.Teachername = (ResultSet["Techername"].ToString());
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
                Command.CommandText = "SELECT c.*, CASE WHEN t.teacherid IS NULL THEN 'Teacher''s code is not valid' ELSE CONCAT(t.teacherfname, ' ', t.teacherlname) END AS Techername FROM courses c LEFT JOIN teachers t ON c.teacherid = t.teacherid WHERE c.courseid = @id;";
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
        /// <summary>
        /// Adds a new course to the database.
        /// </summary>
        /// <param name="CourseData">Course object containing course details to be added.</param>
        /// <example>
        /// POST: api/Course/AddCourse
        /// Headers: Content-Type: application/json
        /// Request Body:
        /// {
        ///     "CourseCode": "CS102",
        ///     "TeacherId": 2,
        ///     "StartDate": "2023-09-01",
        ///     "FinishDate": "2024-01-31",
        ///     "CourseName": "Advanced Programming"
        /// } 
        /// -> 12
        /// </example>
        /// <returns>
        /// The ID of the inserted course if successful. Returns 0 if the operation fails.
        /// </returns>
        [HttpPost(template: "AddCourse")]
        public int AddCourse([FromBody] Course CourseData)
        {
            // 'using' will close the connection after the code executes
            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();
                //Establish a new command (query) for our database
                MySqlCommand Command = Connection.CreateCommand();

                // CURRENT_DATE() for the author join date in this context
                // Other contexts the join date may be an input criteria!
                Command.CommandText = "insert into courses (coursecode, teacherid, startdate, finishdate, coursename) values (@coursecode, @teacherid, @startdate, @finishdate, @coursename )";
                Command.Parameters.AddWithValue("@coursecode", CourseData.Coursecode);
                Command.Parameters.AddWithValue("@teacherid", CourseData.Teacherid);
                Command.Parameters.AddWithValue("@startdate", CourseData.Startdate);
                Command.Parameters.AddWithValue("@finishdate", CourseData.Finishdate);
                Command.Parameters.AddWithValue("@coursename", CourseData.coursename);

                Command.ExecuteNonQuery();

                return Convert.ToInt32(Command.LastInsertedId);

            }
            // if failure
            return 0;
        }
        /// <summary>
        /// Deletes a course from the database by its ID.
        /// </summary>
        /// <param name="CourseId">Primary key of the course to delete.</param>
        /// <example>
        /// DELETE: api/Course/DeleteCourse/1 -> 1
        /// </example>
        /// <returns>
        /// The number of rows affected by the delete operation. Returns 1 if successful, 0 otherwise.
        /// </returns>
        [HttpDelete(template: "DeleteCourse/{CourseId}")]
        public int DeleteCourse(int CourseId)
        {
            // 'using' will close the connection after the code executes
            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();
                //Establish a new command (query) for our database
                MySqlCommand Command = Connection.CreateCommand();


                Command.CommandText = "DELETE FROM courses WHERE courseid = @id";
                Command.Parameters.AddWithValue("@id", CourseId);
                //return Command.ExecuteNonQuery();
                int rowsAffected = Command.ExecuteNonQuery(); // Returns 1 if successful, 0 if not
                return rowsAffected;
            }
            // if failure
            return 0;
        }
        /// <summary>
        /// Updates a Course in the database. Data is a Course object, request query contains ID
        /// </summary>
        /// <param name="updatedCourse">The updated Course data.</param>
        /// <param name="id">The Course ID primary key</param>
        /// <example>
        /// PUT: api/Course/UpdateCourse/4
        /// Headers: Content-Type: application/json
        /// Request Body:
        /// {
        ///     "Coursename":"Updated Course",
        ///     "Coursecode":"CS202",
        ///     "Startdate":"2024-01-01",
        ///     "Finishdate":"2024-06-01"
        /// }
        /// </example>
        /// <returns>The updated Course object</returns>
        [HttpPut("UpdateCourse/{id}")]
        public IActionResult UpdateCourse(int id, [FromBody] Course updatedCourse)
        {
            if (string.IsNullOrEmpty(updatedCourse.Coursecode) || string.IsNullOrEmpty(updatedCourse.Startdate) || string.IsNullOrEmpty(updatedCourse.Finishdate))
            {
                return BadRequest(new { Message = "Missing required fields for course update." });
            }
            using (MySqlConnection connection = _context.AccessDatabase())
            {
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "UPDATE courses SET coursename = @coursename, coursecode = @coursecode, startdate = @startdate, finishdate = @finishdate WHERE courseid = @id";
                command.Parameters.AddWithValue("@coursename", updatedCourse.coursename);
                command.Parameters.AddWithValue("@coursecode", updatedCourse.Coursecode);
                command.Parameters.AddWithValue("@startdate", updatedCourse.Startdate);
                command.Parameters.AddWithValue("@finishdate", updatedCourse.Finishdate);
                command.Parameters.AddWithValue("@id", id);

                command.ExecuteNonQuery();
            }

            return Ok(new { Message = "Course updated successfully." });
        }

    }
}
