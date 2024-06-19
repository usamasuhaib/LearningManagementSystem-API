using System.ComponentModel.DataAnnotations;

namespace School_Management_API.DTOs.Requests
{
    public class TeacherRegisterDto
    {
        [Required]
        public string FullName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        public DateOnly DateOfBirth { get; set; }


        [Required]
        public string Qualification { get; set; }

        public DateOnly HireDate { get; set; }

        public int MobileNumber { get; set; }

        public string Address { get; set; }


    }
}
