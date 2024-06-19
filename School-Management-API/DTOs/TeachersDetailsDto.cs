using System.ComponentModel.DataAnnotations;

namespace School_Management_API.DTOs
{
    public class TeachersDetailsDto
    {

        public int Id { get; set; }
       public string UserId { get; set; }

        public string TeacherId { get; set; }
        public string FullName { get; set; }

  
        public string Email { get; set; }


       
        public string Gender { get; set; }

        public DateOnly DateOfBirth { get; set; }


        public string Qualification { get; set; }

        public DateOnly HireDate { get; set; }

        public int MobileNumber { get; set; }

        public string Address { get; set; }
    }
}
