using System.ComponentModel.DataAnnotations;

namespace School_Management_API.Models
{
    public class Class
    {
        [Key]
        public int Id { get; set; }
        public string ClassName { get; set; }



        // Navigation property
        public ICollection<Subject> Subjects { get; set; }
        public ICollection<Student> Students { get; set; }




    }
}
