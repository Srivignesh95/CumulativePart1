using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace test1.Models
{
    public class Course
    {
        [JsonIgnore]
        public int Courseid { get; set; }
        public string Coursecode { get; set; }
        public int Teacherid { get; set; }
        public string Startdate { get; set; }
        public string Finishdate { get; set; }
        public string coursename { get; set; }
        public string Teachername { get; set; }
    }
}
