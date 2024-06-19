using Microsoft.AspNetCore.Http;
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
    public class SubjectController : ControllerBase
    {
        private readonly SchoolDbContext _dbContext;

        public SubjectController(SchoolDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        [HttpPost("AddSubject")]
        public async Task<IActionResult> AddSubject([FromBody] SubjectDto subjectDto)
        {
            if (ModelState.IsValid)
            {

                var newSubject = new Subject
                {
                    SubjectName = subjectDto.SubjectName,
                };
                await _dbContext.Subjects.AddAsync(newSubject);
                await _dbContext.SaveChangesAsync();

                return Ok(new SubjectResponse
                {
                    Success = true,
                    Result = "New Subject Added Successfully"

                });

            }
            return BadRequest(new SubjectResponse
            {
                Success = false,
                Result = "Failed to add new Subject"
            });
        }




        [HttpGet("SubjectsList")]
        public async Task<IActionResult> SubjectsList()
        {
            var subjectsList = await _dbContext.Subjects.ToListAsync();

            return Ok(subjectsList);

        }


        [HttpPut("AssignClassToSubject")]

        public async Task<IActionResult> AssignClassToSubject([FromBody] AssignClassToSubjectDto assignClassToSubjectDto)
        {
            var subject = await _dbContext.Subjects.FindAsync(assignClassToSubjectDto.SubjectId);
            if (subject is not null)
            {
                subject.ClassId = assignClassToSubjectDto.ClassId;
                _dbContext.Subjects.Update(subject);
                await _dbContext.SaveChangesAsync();
            }

            return Ok();
        }


        [HttpPut("AssignTeacherToSubject")]

        public async Task<IActionResult> AssignTeacherToSubject([FromBody] AssignTeacherToSubjectDto assignTeacherToSubjectDto)
        {
            var subject = await _dbContext.Subjects.FindAsync(assignTeacherToSubjectDto.SubjectId);
            if (subject is not null)
            {
                subject.TeacherId = assignTeacherToSubjectDto.TeacherId;
                _dbContext.Subjects.Update(subject);
                await _dbContext.SaveChangesAsync();
            }
            return Ok();
        }








        [HttpDelete("DeleteSubject/{id}")]
        public async Task<IActionResult> DeleteSubject(int id)
        {

            var subjectExist = await _dbContext.Subjects.FindAsync(id);

            if (subjectExist == null)
            {
                return BadRequest(new SubjectResponse
                {
                    Result = "Delete failed",
                    Success = false,
                });
            }

            _dbContext.Subjects.Remove(subjectExist);
            _dbContext.SaveChanges();
            return Ok(new SubjectResponse { Result = "Delete success", Success = true });
        }
    }
}
