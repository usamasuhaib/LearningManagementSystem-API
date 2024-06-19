using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using School_Management_API.Data;
using School_Management_API.DTOs;
using School_Management_API.Models;
using System;
using System.Security.Claims;

namespace School_Management_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SchoolDbContext _dbContext;

        public StudentController(UserManager<AppUser> userManager, SchoolDbContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }


        [HttpGet("StudentsCount")]
        public async Task<IActionResult> StudentsCount()
        {

            var totalStudents = await _dbContext.Students.CountAsync();
            return Ok(totalStudents);
        }


        [HttpGet("GetStudents")]
        public async Task<IActionResult> GetStudents()
        {
            var students = _userManager.GetUsersInRoleAsync("Student").Result.ToList();

            if (students.Any())
            {
                return Ok(students);
            }
            else
            {
                return NotFound("No students found.");
            }
        }


        [HttpGet("GetStudentsList")]
        public async Task<IActionResult> GetStudentsList()
        {
            // Fetch users with the role "Studnet"
            var studentsInRole = await _userManager.GetUsersInRoleAsync("Student");
            var stidentIds = studentsInRole.Select(t => t.Id).ToList();


            // Fetch Student details from the Studnersw table
            var students = await _dbContext.Students
                .Where(t => stidentIds.Contains(t.AppUserId))
                .Include(t => t.User) // Include AppUser details
                .Include(t => t.Class) // Include Class details
                .Select(t => new StudentsDetailsDto
                {
                    UserId = t.AppUserId,
                    RegistrationNumber = t.RegistrationNumber,
                    FullName = t.User.FullName,
                    Email = t.User.Email,
                    MobileNumber = t.User.MobileNumber,
                    Gender = t.User.Gender,
                    DateOfBirth = t.User.DateOfBirth,
                    Address = t.User.Address,
                    GradeLevel = t.Class.ClassName // Fetch className from the associated Class entity

                })
                .ToListAsync();

            if (students.Any())
            {
                return Ok(students);
            }
            else
            {
                return NotFound("No Students found.");
            }
        }

        [HttpGet("GetStudentDetails/{userId}")]
        public async Task<IActionResult> GetStudentDetails(string userId)
        {
            // Fetch the user with the role "Student"
            var student = await _userManager.FindByIdAsync(userId);

            if (student == null || !await _userManager.IsInRoleAsync(student, "Student"))
            {
                return NotFound("Student not found.");
            }

            // Fetch student details from the Students table
            var studentDetails = await _dbContext.Students
                .Where(s => s.AppUserId == userId)
                .Include(s => s.User) // Include AppUser details
                .Select(s => new StudentsDetailsDto
                {
                    UserId = s.AppUserId,
                    RegistrationNumber = s.RegistrationNumber,
                    FullName = s.User.FullName,
                    Email = s.User.Email,
                    MobileNumber = s.User.MobileNumber,
                    Gender = s.User.Gender,
                    DateOfBirth = s.User.DateOfBirth,
                    Address = s.User.Address,
                })
                .FirstOrDefaultAsync();

            if (studentDetails != null)
            {
                return Ok(studentDetails);
            }
            else
            {
                return NotFound("Student details not found.");
            }
        }



        [HttpGet("GetStudent/{id}")] // Replace '{id}' with actual parameter name
        public async Task<IActionResult> GetStudentById(string id) // Replace 'string' with actual ID type
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Please provide a valid student ID.");
            }

            var student = await _userManager.FindByIdAsync(id);

            if (student == null)
            {
                return NotFound("Student not found.");
            }

            // Check if the user has the "Student" role (optional)
            if (!await _userManager.IsInRoleAsync(student, "Student"))
            {
                return NotFound("User with the provided ID is not a student.");
            }

            return Ok(student); // Consider returning only relevant student data
        }


        [HttpGet("GetStudentSubjects")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetStudentSubjects()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var student = await _dbContext.Students
                .Include(s => s.Class) 
                .ThenInclude(c => c.Subjects) 
                .ThenInclude(sub => sub.Teacher) 
                .ThenInclude(teacher => teacher.User) 
                .FirstOrDefaultAsync(s => s.AppUserId == userId);

            if (student == null)
            {
                return NotFound("Student not found for the current user.");
            }

            var studentSubjects = student.Class.Subjects
                .Select(subject => new
                {
                    SubjectId = subject.Id,
                    SubjectName = subject.SubjectName,
                    //TeacherName=subject.Teacher,
                    TeacherName = subject.Teacher != null ? subject.Teacher.User.FullName : "No Teacher Assigned"
                })
                .ToList();

            //var subjectCount = studentSubjects.Count;

            return Ok(studentSubjects);

        }

        [HttpGet("GetStudentByRegistrationNumber/{registrationNumber}")]
        public async Task<IActionResult> GetStudentByRegistrationNumber(string registrationNumber)
        {
            if (string.IsNullOrEmpty(registrationNumber))
            {
                return BadRequest("Please provide a valid student registration number.");
            }


            var studentRegistration = await _dbContext.Students.FirstOrDefaultAsync(sr => sr.RegistrationNumber == registrationNumber);

            if (studentRegistration == null)
            {
                return NotFound("Student with the provided registration number not found.");
            }

            return Ok(studentRegistration); // Consider returning only relevant student data    
        }



        [HttpDelete("DeleteStudent/{id}")]
        public async Task<IActionResult> DeleteStudent(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Please provide a valid Studnet ID.");
            }

            var student = await _userManager.FindByIdAsync(id);

            if (student == null)
            {
                return NotFound("student with the provided ID not found.");
            }

            // Consider additional checks before deletion (optional)
            // ... (e.g., check if teacher has assigned courses)

            await _userManager.DeleteAsync(student);

            return Ok(); // Consider returning "No Content" (204) after successful deletion
        }

    }
}
