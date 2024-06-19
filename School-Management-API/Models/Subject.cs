using System.ComponentModel.DataAnnotations;

namespace School_Management_API.Models
{
    public class Subject
    {
        [Key]
        public int Id { get; set; }
        public string SubjectName { get; set; }



        // Foreign key to Class (nullable to allow independent registration)
        public int? ClassId { get; set; }
        public Class Class { get; set; }



        // Foreign key to Teacher (nullable to allow independent registration)
        public int? TeacherId { get; set; }
        public Teacher Teacher { get; set; } // One-to-many relationship with Teacher
    }
}
