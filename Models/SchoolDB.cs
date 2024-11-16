using MySql.Data.MySqlClient;

namespace test1.Models
{
    public class SchoolDB
    {
        // These are readonly "secret" properties. 
        // Only the SchoolDB class can use them.
        // Change these to match your own local school database configuration!
        private static string User { get { return "root"; } }
        private static string Password { get { return ""; } }
        private static string Database { get { return "school"; } }
        private static string Server { get { return "localhost"; } }
        private static string Port { get { return "3306"; } }

        // ConnectionString is a series of credentials used to connect to the database.
        protected static string ConnectionString
        {
            get
            {
                // "convert zero datetime" is a database connection setting that returns NULL if the date is 0000-00-00.
                // This allows C# to interpret the absence of a date as NULL instead of an invalid date.

                return "server = " + Server
                    + "; user = " + User
                    + "; database = " + Database
                    + "; port = " + Port
                    + "; password = " + Password
                    + "; convert zero datetime = True";
            }
        }
        // This is the method we actually use to get the database connection!
        /// <summary>
        /// Returns a connection to the school database.
        /// </summary>
        /// <example>
        /// private SchoolDB School = new SchoolDB();
        /// MySqlConnection Conn = School.AccessDatabase();
        /// </example>
        /// <returns>A MySqlConnection Object</returns>
        public MySqlConnection AccessDatabase()
        {
            //We are instantiating the MySqlConnection Class to create an object
            //the object is a specific connection to our blog database on port 3307 of localhost
            return new MySqlConnection(ConnectionString);
        }
    }
}
