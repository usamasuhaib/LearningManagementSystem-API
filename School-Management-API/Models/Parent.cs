using System.ComponentModel.DataAnnotations;

namespace School_Management_API.Models
{
    public class Parent
    {
        [Key]
        public int Id { get; set; } // Primary key for Admin table
        public string AppUserId { get; set; } // Foreign key referencing AppUser
        public AppUser User { get; set; } // Navigation property for one-to-one relationship


        public ICollection<Student> Students { get; set; } // Navigation property for one-to-many (one parent, many students)


    }
}
