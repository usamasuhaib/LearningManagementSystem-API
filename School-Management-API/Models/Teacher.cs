using System.ComponentModel.DataAnnotations;

namespace School_Management_API.Models
{
    public class Teacher
    {
        [Key]
        public int Id { get; set; }

        public string TeacherId { get; set; } // 
        public string Qualification { get; set; }
        public DateOnly HireDate { get; set; }

        //GUID
        public string AppUserId { get; set; } // Foreign key referencing AppUser
        public AppUser User { get; set; } // Navigation property for one-to-one relationship


        public ICollection<Subject> Subjects { get; set; } // One-to-many relationship with Subject

    }
}
