using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using School_Management_API.Configurations;
using School_Management_API.Data;
using School_Management_API.DTOs.Requests;
using School_Management_API.DTOs.Responses;
using School_Management_API.Helpers;
using School_Management_API.Models;
using School_Management_API.Responses;
using School_Management_API.Services.AuthService;
using System.Web;

namespace School_Management_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IAuthService _authService;
        private readonly SchoolDbContext _schoolDb;

        public AuthenticationController(UserManager<AppUser> userManager, IAuthService authService, SchoolDbContext schoolDb)
        {
            _userManager = userManager;
            _authService = authService;
            _schoolDb = schoolDb;
        }


        //admin registration 
        [HttpPost("AdminRegister")]
        public async Task<IActionResult> AdminRegister(AdminRegisterDto adminDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthResult { Success = false, Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList() });
            }

            try
            {
                var emailConfirmationToken = await _authService.AdminRegister(adminDto);
                if (emailConfirmationToken is not null)
                {
                    var confirmationLink = Url.Action(nameof(ConfirmEmail), "Authentication", new { token = emailConfirmationToken, email = adminDto.Email }, Request.Scheme);
                    var userEmail = adminDto.Email;
                    var fullName = adminDto.FullName;
                    var subject = "Email Confirmation Link";
                    var emailBody = EmailBody.adminEmailConfirmBody(fullName, confirmationLink);
                    await _authService.SendEmailAsync(userEmail, subject, emailBody);
                }

                return Ok(new AuthResult { Success = true, Result = "Registration successful. Please check your email for confirmation." });
            }
            catch (Exception ex)
            {
                return BadRequest(new AuthResult { Success = false, Errors = new List<string> { "Registration failed. Please try again later.", ex.Message } });
            }
        }



        //teacher registration 

        [HttpPost("TeacherRegister")]

        public async Task<IActionResult> TeacherRegister(TeacherRegisterDto teacherDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthResult { Success = false, Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList() });
            }

            try
            {
                var emailConfirmationToken = await _authService.TeacherRegister(teacherDto);
                if (emailConfirmationToken != null)
                {
                    var confirmationLink = Url.Action(nameof(ConfirmEmail), "Authentication", new { token = emailConfirmationToken, email = teacherDto.Email }, Request.Scheme);
                    var userEmail = teacherDto.Email;
                    var fullName = teacherDto.FullName;
                    var subject = "Email Confirmation Link";
                    var emailBody = EmailBody.tacherEmailConfirmBody(fullName, confirmationLink);
                    await _authService.SendEmailAsync(userEmail, subject, emailBody);
                }

                return Ok(new AuthResult { Success = true, Result = "Registration successful" });
            }
            catch (Exception ex)
            {
                return BadRequest(new AuthResult { Success = false, Errors = new List<string> { "Registration failed. Please try again later.", ex.Message } });
            }
        }



        //teacher registration 

        [HttpPost("StudentRegister")]

        public async Task<IActionResult> StudentRegister(StudentRegisterDto studentDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthResult { Success = false, Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList() });
            }

            try
            {
                var emailConfirmationToken = await _authService.StudentRegister(studentDto);
                if (emailConfirmationToken is not null)
                {
                    var confirmationLink = Url.Action(nameof(ConfirmEmail), "Authentication", new { token = emailConfirmationToken, email = studentDto.Email }, Request.Scheme);
                    var userEmail = studentDto.Email;
                    var fullName = studentDto.FullName;
                    var subject = "Email Confirmation Link";
                    var emailBody = EmailBody.studnetEmailConfirmBody(fullName, confirmationLink);
                    await _authService.SendEmailAsync(userEmail, subject, emailBody);
                }

                return Ok(new AuthResult { Success = true, Result = "Registration successful." });
            }
            catch (Exception ex)
            {
                return BadRequest(new AuthResult { Success = false, Errors = new List<string> { "Registration failed. Please try again later.", ex.Message } });
            }
        }





        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userExist = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);

                    if (userExist is not null)
                    {
                        if (!userExist.EmailConfirmed)
                        {
                            return BadRequest(new LoginResponse
                            {
                                Success = false,
                                Result = "Login failed",
                                Errors = new List<string> { "Email Not confirmed . Please confirm your email first." }
                            });

                        }

                        else
                        {
                            var passwordMatch = await _userManager.CheckPasswordAsync(userExist, loginDto.Password);

                            if (passwordMatch)
                            {
                                var token = await _authService.Login(loginDto);

                                return Ok(new LoginResponse
                                {
                                    Success = true,
                                    Result = "Login successful",
                                    Token = token
                                });
                            }
                            else
                            {
                                return BadRequest(new LoginResponse
                                {
                                    Success = false,
                                    Result = "Login failed",
                                    Errors = new List<string> { "Invalid password. Please try again." }
                                });
                            }

                        }

                    }
                    else
                    {
                        return BadRequest(new LoginResponse
                        {
                            Success = false,
                            Result = "Login failed",
                            Errors = new List<string> { "User not found . Please register your email." }
                        });
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(new LoginResponse
                    {
                        Success = false,
                        Result = "Login failed",
                        Errors = new List<string> { ex.Message }
                    });
                }
            }

            return BadRequest(new LoginResponse
            {
                Success = false,
                Result = "Login failed",
                Errors = new List<string> { "Invalid payload" }
            });
        }

        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user is not null)
                {
                    var result = await _userManager.ConfirmEmailAsync(user, token);
                    if (result.Succeeded)
                    {
                        var jwtToken = await _authService.GenerateJwtToken(user);
                        var message = $"Welcome {user.FullName} Your Email is verified Successfully";
                        string encodedMessage = HttpUtility.UrlEncode(message);

                        // Redirect to user dashboard

                        return Ok(new RegisterResponse
                        {
                            Token = jwtToken,
                            Success = true,
                            Result = message,
                        });

                    }
                }
                return BadRequest(new ConfirmEmailResponse { Success = false, Result = "Email confirmation failed" });
            }
            catch (Exception ex)
            {
                // Log the exception details for troubleshooting
                return StatusCode(500, new ConfirmEmailResponse
                {
                    Success = false,
                    Result = "Internal Server Error",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
    }

}
