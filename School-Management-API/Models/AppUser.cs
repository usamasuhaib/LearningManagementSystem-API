using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;

namespace School_Management_API.Models
{
    public class AppUser :IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;

        public DateOnly DateOfBirth { get; set; }
        public string Address { get; set; } = string.Empty;
        public int MobileNumber { get; set; }




        [JsonIgnore] // Exclude admin property from serialization
        public Admin Admin { get; set; }


        [JsonIgnore]
        public Teacher Teacher { get; set; }


        [JsonIgnore] 
        public Student Student { get; set; }


        [JsonIgnore] 
        public Parent Parent { get; set; }

    }
}
