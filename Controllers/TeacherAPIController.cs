using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;
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
        
        [HttpGet(template: "FindTeachersByHireDate")]
        public List<Teacher> FindTeachersByHireDate(DateTime minDate, DateTime maxDate)
        {
            List<Teacher> teachers = new List<Teacher>();

            using (MySqlConnection connection = _context.AccessDatabase())
            {
                connection.Open();
                MySqlCommand command = connection.CreateCommand();

                command.CommandText = @"
            SELECT * 
            FROM teachers 
            WHERE hiredate >= @minDate AND hiredate <= @maxDate;
        ";
                command.Parameters.AddWithValue("@minDate", minDate);
                command.Parameters.AddWithValue("@maxDate", maxDate);

                using (MySqlDataReader resultSet = command.ExecuteReader())
                {
                    while (resultSet.Read())
                    {
                        Teacher teacher = new Teacher
                        {
                            Teacherid = Convert.ToInt32(resultSet["teacherid"]),
                            Teacherfname = resultSet["teacherfname"].ToString(),
                            Teacherlname = resultSet["teacherlname"].ToString(),
                            Employeenumber = resultSet["employeenumber"].ToString(),
                            Hiredate = resultSet["hiredate"].ToString(),
                            Salary = Convert.ToDecimal(resultSet["salary"])
                        };
                        teachers.Add(teacher);
                    }
                }
            }

            return teachers;
        }
        /// <summary>
        /// Adds a new teacher to the database.
        /// </summary>
        /// <param name="TeacherData">The Teacher object containing the teacher's details.</param>
        /// <example>
        /// POST: api/Teacher/AddTeacher
        /// Headers: Content-Type: application/json
        /// Request Body:
        /// {
        ///     "Teacherfname":"John",
        ///     "Teacherlname":"Doe",
        ///     "Employeenumber":"T12345",
        ///     "Hiredate":"2023-01-01",
        ///     "Salary":50000
        /// }
        /// </example>
        [HttpPost(template: "AddTeacher")]
        public IActionResult AddTeacher([FromBody] Teacher TeacherData)
        {
            string pattern = @"^T\d+$";

            if (!Regex.IsMatch(TeacherData.Employeenumber, pattern))
            {
                // Return error message for invalid format
                return BadRequest(new { Message = "Invalid Employee Number. It must start with 'T' followed by digits." });
            }
            try
            {

                // 'using' will close the connection after the code executes
                using (MySqlConnection Connection = _context.AccessDatabase())
                {
                    Connection.Open();

                    // Check for duplicate employee number
                    MySqlCommand checkCommand = Connection.CreateCommand();
                    checkCommand.CommandText = "SELECT COUNT(*) FROM teachers WHERE employeenumber = @employeenumber";
                    checkCommand.Parameters.AddWithValue("@employeenumber", TeacherData.Employeenumber);

                    int count = Convert.ToInt32(checkCommand.ExecuteScalar());
                    if (count > 0)
                    {
                        // Return error if duplicate found
                        return BadRequest(new { Message = "Employee number already exists. Please use a different one." });
                    }

                    //Establish a new command (query) for our database
                    MySqlCommand Command = Connection.CreateCommand();

                    // CURRENT_DATE() for the author join date in this context
                    // Other contexts the join date may be an input criteria!
                    Command.CommandText = "insert into teachers (teacherfname, teacherlname, employeenumber, hiredate, salary) values (@teacherfname, @teacherlname, @employeenumber, @hiredate, @salary )";
                    Command.Parameters.AddWithValue("@teacherfname", TeacherData.Teacherfname);
                    Command.Parameters.AddWithValue("@teacherlname", TeacherData.Teacherlname);
                    Command.Parameters.AddWithValue("@employeenumber", TeacherData.Employeenumber);
                    Command.Parameters.AddWithValue("@hiredate", TeacherData.Hiredate);
                    Command.Parameters.AddWithValue("@salary", TeacherData.Salary);

                    Command.ExecuteNonQuery();

                    int teacherId = Convert.ToInt32(Command.LastInsertedId);

                    return Ok(new { TeacherId = teacherId, Message = "Teacher added successfully." });

                }
            }
            catch (Exception ex)
            {
                // Return generic error message for unexpected errors
                return StatusCode(500, new { Message = "An unexpected error occurred.", Details = ex.Message });
            }
        }
        /// <summary>
        /// Deletes a teacher from the database by their ID.
        /// </summary>
        /// <param name="TeacherId">The ID of the teacher to delete.</param>
        /// <example>
        /// DELETE: api/Teacher/DeleteTeacher/5 -> 1
        /// </example>
        /// <returns>The number of rows affected by the delete operation (1 if successful, 0 otherwise).</returns>

        [HttpDelete(template: "DeleteTeacher/{TeacherId}")]
        public int DeleteTeacher(int TeacherId)
        {
            // 'using' will close the connection after the code executes
            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();
                //Establish a new command (query) for our database
                MySqlCommand Command = Connection.CreateCommand();


                Command.CommandText = "DELETE FROM teachers WHERE teacherid = @id";
                Command.Parameters.AddWithValue("@id", TeacherId);
                //return Command.ExecuteNonQuery();
                int rowsAffected = Command.ExecuteNonQuery(); // Returns 1 if successful, 0 if not
                return rowsAffected;
            }
            // if failure
            return 0;
        }
        /// <summary>
        /// Updates an Author in the database. Data is Author object, request query contains ID
        /// </summary>
        /// <param name="AuthorData">Author Object</param>
        /// <param name="AuthorId">The Author ID primary key</param>
        /// <example>
        /// PUT: api/Author/UpdateAuthor/4
        /// Headers: Content-Type: application/json
        /// Request Body:
        /// {
        ///	    "AuthorFname":"Christine",
        ///	    "AuthorLname":"Bittle",
        ///	    "AuthorBio":"Likes Coding!",
        ///	    "AuthorEmail":"christine@test.ca"
        /// } -> 
        /// {
        ///     "AuthorId":4,
        ///	    "AuthorFname":"Christine",
        ///	    "AuthorLname":"Bittle",
        ///	    "AuthorBio":"Likes Coding!",
        ///	    "AuthorEmail":"christine@test.ca"
        /// }
        /// </example>
        /// <returns>
        /// The updated Author object
        /// </returns>
        [HttpPut(template: "UpdateTeacher/{TeacherId}")]
        public IActionResult UpdateTeacher(int TeacherId, [FromBody] Teacher TeacherData)
        {
            try
            {
                // 'using' will close the connection after the code executes
                using (MySqlConnection Connection = _context.AccessDatabase())
                {
                    Connection.Open();

                    //Establish a new command (query) for our database
                    MySqlCommand Command = Connection.CreateCommand();
                    Command.CommandText = "SELECT COUNT(*) FROM teachers WHERE teacherid = @id";
                    Command.Parameters.AddWithValue("@id", TeacherId);
                    int count = Convert.ToInt32(Command.ExecuteScalar());
                    if (count == 0)
                    {
                        // Return 404 if the teacher does not exist
                        return NotFound(new { Message = "The teacher you are trying to update does not exist." });
                    }
                    MySqlCommand updateCommand = Connection.CreateCommand();
                    // parameterize query
                    updateCommand.CommandText = "update teachers set teacherfname=@teacherfname, teacherlname=@teacherlname, employeenumber=@employeenumber, hiredate=@hiredate,salary=@salary where teacherid=@id";
                    updateCommand.Parameters.AddWithValue("@teacherfname", TeacherData.Teacherfname);
                    updateCommand.Parameters.AddWithValue("@teacherlname", TeacherData.Teacherlname);
                    updateCommand.Parameters.AddWithValue("@employeenumber", TeacherData.Employeenumber);
                    updateCommand.Parameters.AddWithValue("@hiredate", TeacherData.Hiredate);
                    updateCommand.Parameters.AddWithValue("@salary", TeacherData.Salary);

                    updateCommand.Parameters.AddWithValue("@id", TeacherId);

                    updateCommand.ExecuteNonQuery();

                    return Ok(new { Message = "Teacher updated successfully." });

                }
            }

            catch (Exception ex)
            {
                // Return 500 for unexpected errors
                return StatusCode(500, new { Message = "An unexpected error occurred.", Details = ex.Message });
            }
        }
    }
}
