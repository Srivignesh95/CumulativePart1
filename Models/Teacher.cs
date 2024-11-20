namespace test1.Models
{
    public class Teacher
    {
        public int Teacherid { get; set; }
        public string Teacherfname { get; set; }
        public string Teacherlname { get; set; }
        public string Employeenumber { get; set; }
        public string Hiredate { get; set; }
        public decimal Salary { get; set; }
        public List<Course> Coursename { get; set; } = new List<Course>();
    }
}
