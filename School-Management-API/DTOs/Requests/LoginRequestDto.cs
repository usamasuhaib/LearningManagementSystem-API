using System.ComponentModel.DataAnnotations;

namespace School_Management_API.DTOs.Requests
{
    public class LoginRequestDto
    {



        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
