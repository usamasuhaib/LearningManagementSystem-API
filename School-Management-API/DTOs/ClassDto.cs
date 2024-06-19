using System.ComponentModel.DataAnnotations;

namespace School_Management_API.DTOs
{
    public class ClassDto
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ClassName { get; set; }


        public int? NumberOfStudents { get; set; }
    }
}
