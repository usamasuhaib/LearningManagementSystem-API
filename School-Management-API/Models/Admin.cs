namespace School_Management_API.Models
{
    public class Admin
    {
        public int Id { get; set; } // Primary key for Admin table
        public DateTime HireDate { get; set; }
        public DateOnly DOB { get; set; }

        public string AppUserId { get; set; } // Foreign key referencing AppUser
        public AppUser User { get; set; } // Navigation property for one-to-one relationship
    }
}
