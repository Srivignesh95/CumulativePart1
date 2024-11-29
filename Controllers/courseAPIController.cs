﻿using Microsoft.AspNetCore.Mvc;
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
        /// Adds an author to the database
        /// </summary>
        /// <param name="AuthorData">Author Object</param>
        /// <example>
        /// POST: api/AuthorData/AddAuthor
        /// Headers: Content-Type: application/json
        /// Request Body:
        /// {
        ///	    "AuthorFname":"Christine",
        ///	    "AuthorLname":"Bittle",
        ///	    "AuthorBio":"Likes Coding!",
        ///	    "AuthorEmail":"christine@test.ca"
        /// } -> 409
        /// </example>
        /// <returns>
        /// The inserted Author Id from the database if successful. 0 if Unsuccessful
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
        /// Deletes an Author from the database
        /// </summary>
        /// <param name="AuthorId">Primary key of the author to delete</param>
        /// <example>
        /// DELETE: api/AuthorData/DeleteAuthor -> 1
        /// </example>
        /// <returns>
        /// Number of rows affected by delete operation.
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

    }
}
