using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using School_Management_API.Data;
using School_Management_API.DTOs;
using School_Management_API.Models;
using School_Management_API.Responses;

namespace School_Management_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassController : ControllerBase
    {
        private readonly SchoolDbContext _dbContext;

        public ClassController(SchoolDbContext dbContext)
        {
            _dbContext = dbContext;
        }


  

        [HttpGet("ClassList")]
        public async Task<IActionResult> ClassList()
        {
            var classList = await _dbContext.Classes
                .Select(c => new
                {
                    ClassId = c.Id,
                    ClassName = c.ClassName,
                    NumberOfStudents = c.Students.Count() // Count the number of students associated with each class
                })
                .ToListAsync();

            return Ok(classList);
        }



        [HttpGet("GetClassById/{id}")]
        public async Task<IActionResult> GetClassById(int id)
        {
            var classDetails = await _dbContext.Classes
      .Include(c => c.Students) // Include students associated with the class
      .Include(c => c.Subjects) // Include subjects associated with the class
      .ThenInclude(s => s.Teacher) // Include the teacher associated with each subject
      .Where(c => c.Id == id)
      .Select(c => new
      {
          ClassId = c.Id,
          ClassName = c.ClassName,
          NumberOfStudents = c.Students.Count(), // Count the number of students associated with this class
          Students = c.Students.Select(s => new
          {
              StudentId = s.Id,
              StudentRegistration = s.RegistrationNumber,
              StudentName = s.User.FullName
          }),
          Subjects = c.Subjects.Select(s => new
          {
              SubjectId = s.Id,
              SubjectName = s.SubjectName,
              Teacher = s.Teacher != null ? new
              {
                  TeacherId = s.Teacher.Id,
                  TeacherName = s.Teacher.User.FullName
              } : null
          }).ToList()
      })
      .FirstOrDefaultAsync();


            if (classDetails is not null)
            {
                return Ok(classDetails);
            }

            return NotFound(new ClassResponse
            {
                Success = false,
                Result = "Class not found"
            });
        }




        [HttpPost("AddClass")]
        public async Task<IActionResult> AddClass([FromBody] ClassDto classDto)
        {
            if (ModelState.IsValid)
            {
             
                var newClass = new Class
                {
                    ClassName=classDto.ClassName,
                };
                await _dbContext.Classes.AddAsync(newClass);
                await _dbContext.SaveChangesAsync();

                return Ok(new ClassResponse
                {
                    Success=true,
                    Result="New Class Added Successfully"

                });

            }
            return BadRequest(new ClassResponse
            {
                Success = false,
                Result = "Failed to add new class"
            });
        }



        [HttpDelete("DeleteClass/{id}")]
        public async Task<IActionResult> DeleteClass(int id)
        {

            var classExist = await _dbContext.Classes.FindAsync(id);

            if (classExist == null)
            {
                return BadRequest(new ClassResponse
                {
                    Result = "Delete failed",
                    Success = false,
                });
            }

            _dbContext.Classes.Remove(classExist);
            _dbContext.SaveChanges();
            return Ok(new ClassResponse { Result="Delete success", Success=true});
        }
    }
}