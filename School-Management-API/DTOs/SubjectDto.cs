using System.ComponentModel.DataAnnotations;

namespace School_Management_API.DTOs
{
    public class SubjectDto
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string SubjectName { get; set; }

    }
}
