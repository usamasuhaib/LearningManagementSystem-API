using School_Management_API.DTOs.Requests;
using School_Management_API.Models;

namespace School_Management_API.Services.AuthService
{
    public interface IAuthService
    {
        public Task<string> AdminRegister(AdminRegisterDto adminDto);

        public Task<string> TeacherRegister(TeacherRegisterDto teacherDto);
        public Task<string> StudentRegister(StudentRegisterDto studentDto);



        public Task<string> GenerateJwtToken(AppUser user);

        public Task SendEmailAsync(string email, string subject, string emailbody);


        public Task<string> Login(LoginRequestDto user);


    }
}
