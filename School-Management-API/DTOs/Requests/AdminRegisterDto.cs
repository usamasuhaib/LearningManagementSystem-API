using System.ComponentModel.DataAnnotations;

namespace School_Management_API.DTOs.Requests
{

    public class AdminRegisterDto
    {
  
        [Required]
        public string FullName { get; set; }

        [Required]
        public string Email { get; set; }



        [Required]
        public string Password { get; set; }
    }
}