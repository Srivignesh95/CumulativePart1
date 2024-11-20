namespace test1.Models
{
    public class Students
    {
        public int Studentid { get; set; }
        public string Studentfname { get; set; }
        public string Studentlname { get; set; }
        public string Studentnumber { get; set; }
        public string Enroledate { get; set; }
        public List<Course> Coursesname { get; set; } = new List<Course>();
    }
}
