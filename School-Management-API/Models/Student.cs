using System.ComponentModel.DataAnnotations;

namespace School_Management_API.Models
{
    public class Student
    {
        [Key]
        public int Id { get; set; }

        public string RegistrationNumber { get; set; } // 
        public string GradeLevel { get; set; }



        public string AppUserId { get; set; } // Foreign key referencing AppUser
        public AppUser User { get; set; } // Navigation property for one-to-one relationship



        // Foreign key for Class
        public int ClassId { get; set; }
        // Navigation property for the one-to-many relationship
        public Class Class { get; set; }



        //public int ParentId { get; set; } // Foreign key referencing Parent
        //public Parent Parent { get; set; } // Navigation property for one-to-many
    }
}
