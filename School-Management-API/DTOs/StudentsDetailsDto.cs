namespace School_Management_API.DTOs
{
    public class StudentsDetailsDto
    {
        public string UserId { get; set; }

        public string RegistrationNumber { get; set; }

        public string GradeLevel { get; set; }

        public string FullName { get; set; }


        public string Email { get; set; }



        public string Gender { get; set; }

        public DateOnly DateOfBirth { get; set; }


        public int MobileNumber { get; set; }

        public string Address { get; set; }
    }
}
