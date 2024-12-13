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
                        //CurrentStudents.Coursesname.Add(ResultSet["coursename"].ToString());
                        Course CurrentCourse = new Course
                        {
                            coursename = ResultSet["coursename"].ToString(),
                        };

                        // Add the course to the teacher's course list
                        CurrentStudents.Coursesname.Add(CurrentCourse);

                    }
                }
            }


            // Return the student object with associated courses
            return CurrentStudents;
        }
        /// <summary>
        /// Adds a new student to the database.
        /// </summary>
        /// <param name="StudentData">Student object containing details to be added.</param>
        /// <example>
        /// POST: api/Students/AddStudent
        /// Headers: Content-Type: application/json
        /// Request Body:
        /// {
        ///	    "Studentfname":"John",
        ///	    "Studentlname":"Doe",
        ///	    "Studentnumber":"T12345",
        ///	    "Enroldate":"2024-01-01"
        /// } -> Returns the ID of the added student.
        /// </example>
        /// <returns>
        /// The inserted Student ID from the database if successful, or 0 if unsuccessful.
        /// </returns>
        [HttpPost(template: "AddStudent")]
        public int AddStudent([FromBody] Students StudentData)
        {
            // 'using' will close the connection after the code executes
            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();
                //Establish a new command (query) for our database
                MySqlCommand Command = Connection.CreateCommand();

                // CURRENT_DATE() for the author join date in this context
                // Other contexts the join date may be an input criteria!
                Command.CommandText = "insert into students (studentfname, studentlname, studentnumber, enroldate) values (@studentfname, @studentlname, @studentnumber, @enroldate )";
                Command.Parameters.AddWithValue("@studentfname", StudentData.Studentfname);
                Command.Parameters.AddWithValue("@studentlname", StudentData.Studentlname);
                Command.Parameters.AddWithValue("@studentnumber", StudentData.Studentnumber);
                Command.Parameters.AddWithValue("@enroldate", StudentData.Enroledate);

                Command.ExecuteNonQuery();

                return Convert.ToInt32(Command.LastInsertedId);

            }
            // if failure
            return 0;
        }
        /// <summary>
        /// Deletes a student from the database by their ID.
        /// </summary>
        /// <param name="StudentId">Primary key of the student to delete.</param>
        /// <example>
        /// DELETE: api/Students/DeleteStudent/1 -> 1
        /// </example>
        /// <returns>
        /// The number of rows affected by the delete operation (1 if successful, 0 if unsuccessful).
        /// </returns>
        [HttpDelete(template: "DeleteStudent/{StudentId}")]
        public int DeleteStudent(int StudentId)
        {
            // 'using' will close the connection after the code executes
            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();
                //Establish a new command (query) for our database
                MySqlCommand Command = Connection.CreateCommand();


                Command.CommandText = "DELETE FROM students WHERE studentid = @id";
                Command.Parameters.AddWithValue("@id", StudentId);
                //return Command.ExecuteNonQuery();
                int rowsAffected = Command.ExecuteNonQuery(); // Returns 1 if successful, 0 if not
                return rowsAffected;
            }
            // if failure
            return 0;
        }
        /// <summary>
        /// Updates a Student in the database and returns the updated student object.
        /// </summary>
        /// <param name="updatedStudent">The updated student data.</param>
        /// <param name="id">The Student ID primary key.</param>
        /// <example>
        /// PUT: api/Student/UpdateStudent/4
        /// Headers: Content-Type: application/json
        /// Request Body:
        /// {
        ///     "FirstName": "Updated FirstName",
        ///     "LastName": "Updated LastName",
        ///     "EnrollmentDate": "2024-01-01"
        /// }
        /// </example>
        /// <returns>The updated Student object.</returns>
        [HttpPut("UpdateStudent/{id}")]
        public Students UpdateStudent(int id, [FromBody] Students updatedStudent)
        {
            using (MySqlConnection connection = _context.AccessDatabase())
            {
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "UPDATE students SET studentfname = @studentfname, studentlname = @studentlname, studentnumber= @studentnumber, enroldate = @enroldate WHERE studentid = @id";
                command.Parameters.AddWithValue("@studentfname", updatedStudent.Studentfname);
                command.Parameters.AddWithValue("@studentlname", updatedStudent.Studentlname);
                command.Parameters.AddWithValue("@studentnumber", updatedStudent.Studentnumber);
                command.Parameters.AddWithValue("@enroldate", updatedStudent.Enroledate);
                command.Parameters.AddWithValue("@id", id);

                command.ExecuteNonQuery();
            }

            return FindStudent(id);
        }

    }
}
