using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using School_Management_API.Data;
using School_Management_API.DTOs;
using School_Management_API.DTOs.Requests;
using School_Management_API.Models;
using System;
using System.Security.Claims;

namespace School_Management_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeacherController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SchoolDbContext _dbContext;

        public TeacherController(UserManager<AppUser> userManager, SchoolDbContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }

        [HttpGet("TeachersCount")]
        public async Task<IActionResult> TeachersCount()
        {

            var totalTeachers = await _dbContext.Teachers.CountAsync();
            return Ok(totalTeachers);
        }

        [HttpGet("GetTeachers")]
        public async Task<IActionResult> GetTeachers()
        {
            var teachers = _userManager.GetUsersInRoleAsync("Teacher").Result.ToList();

            if (teachers.Any())
            {
                return Ok(teachers);
            }
            else
            {
                return NotFound("No teachers found.");
            }
        }


        [HttpGet("GetTeachersList")]
        public async Task<IActionResult> GetTeachersList()
        {
            // Fetch users with the role "Teacher"
            var teachersInRole = await _userManager.GetUsersInRoleAsync("Teacher");
            var teacherIds = teachersInRole.Select(t => t.Id).ToList();


            // Fetch teacher details from the Teachers table
            var teachers = await _dbContext.Teachers
                .Where(t => teacherIds.Contains(t.AppUserId))
                .Include(t => t.User) // Include AppUser details
                .Select(t => new TeachersDetailsDto
                {
                    Id=t.Id,
                    UserId = t.AppUserId,
                    TeacherId=t.TeacherId,
                    FullName = t.User.FullName,
                    Email = t.User.Email,
                    MobileNumber = t.User.MobileNumber,
                    Gender = t.User.Gender,
                    DateOfBirth = t.User.DateOfBirth,
                    Address = t.User.Address,
                    Qualification = t.Qualification,
                    HireDate = t.HireDate
                })
                .ToListAsync();

            if (teachers.Any())
            {
                return Ok(teachers);
            }
            else
            {
                return NotFound("No teachers found.");
            }
        }




        [HttpGet("GetTeacherDetails/{userId}")]
        public async Task<IActionResult> GetTeacherDetails(string userId)
        {
            // Fetch the user with the role "Teacher"
            var teacher = await _userManager.FindByIdAsync(userId);

            if (teacher == null || !await _userManager.IsInRoleAsync(teacher, "Teacher"))
            {
                return NotFound("Teacher not found.");
            }

            // Fetch student details from the Students table
            var teacherDetails = await _dbContext.Teachers
                .Where(s => s.AppUserId == userId)
                .Include(s => s.User) // Include AppUser details
                .Select(s => new TeachersDetailsDto
                {
                    UserId = s.AppUserId,
                    TeacherId = s.TeacherId,
                    FullName = s.User.FullName,
                    Email = s.User.Email,
                    MobileNumber = s.User.MobileNumber,
                    Gender = s.User.Gender,
                    DateOfBirth = s.User.DateOfBirth,
                    Address = s.User.Address,
                    Qualification = s.Qualification,
                    HireDate=s.HireDate,
                })
                .FirstOrDefaultAsync();

            if (teacherDetails != null)
            {
                return Ok(teacherDetails);
            }
            else
            {
                return NotFound("Teacher details not found.");
            }
        }




        [HttpGet("GetTeacherSubjects")]
        [Authorize(Roles = "Teacher")] 
        public async Task<IActionResult> GetTeacherSubjects()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var teacher = await _dbContext.Teachers
                .FirstOrDefaultAsync(t => t.AppUserId == userId);

            if (teacher == null)
            {
                return NotFound("Teacher not found for the current user.");
            }

            var teacherSubjects = await _dbContext.Subjects
             .Where(s => s.TeacherId == teacher.Id)
             .Include(s => s.Class)                       
             .Select(s => new
             {
                 SubjectId = s.Id,
                 SubjectName = s.SubjectName,
                 GradeLevel = s.Class.ClassName,
             })
             .ToListAsync();

            var subjectCount = teacherSubjects.Count;

            return Ok(new { Subjects = teacherSubjects, SubjectCount = subjectCount });
        }







        [HttpDelete("DeleteTeacher/{id}")]
        public async Task<IActionResult> DeleteTeacher(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Please provide a valid teacher ID.");
            }

            var teacher = await _userManager.FindByIdAsync(id);

            if (teacher == null)
            {
                return NotFound("Teacher with the provided ID not found.");
            }

            // Consider additional checks before deletion (optional)
            // ... (e.g., check if teacher has assigned courses)

            await _userManager.DeleteAsync(teacher);

            return Ok();
        }




        [HttpGet("GetUserWithTeachers")]
        public async Task<IActionResult> GetUserWithTeachers()
        {
            // Get all users with "Teacher" role
            var users = await _userManager.GetUsersInRoleAsync("Teacher");

            var teacherDetails = new List<TeachersDetailsDto>();  // Use singular name

            foreach (var user in users)
            {
                // Get teacher details (handle potential null)
                var teacher = await _dbContext.Teachers.FindAsync(user.Id);
                if (teacher != null)
                {
                    var teacherViewModel = new TeachersDetailsDto  // Use singular name
                    {
                        FullName = user.FullName,
                        Email = user.Email,
                        // Add other common user properties
                        Qualification = teacher.Qualification,
                        // Add other teacher-specific properties
                    };

                    teacherDetails.Add(teacherViewModel);
                }
                else
                {
                    // Handle case where user doesn't have a teacher entry (log or return specific message)
                    // ...
                }
            }

            return Ok(teacherDetails);
        }





    }
}
