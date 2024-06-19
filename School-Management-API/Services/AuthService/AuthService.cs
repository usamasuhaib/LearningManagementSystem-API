using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using School_Management_API.DTOs.Requests;
using School_Management_API.Models;
using SMS_API.Configurations;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System;

using MailKit.Net.Smtp;
using MimeKit;
using School_Management_API.Data;
using MimeKit.Text;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;
using MailKit.Security;



namespace School_Management_API.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly SchoolDbContext _dbContext;
        private readonly JwtConfig _jwtConfig;


        public AuthService(UserManager<AppUser> userManager, IOptionsMonitor<JwtConfig> optionsMonitor, RoleManager<IdentityRole> roleManager, IConfiguration configuration, SchoolDbContext dbContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _dbContext = dbContext;
            _jwtConfig = optionsMonitor.CurrentValue;

        }

        public async Task<string> AdminRegister(AdminRegisterDto adminDto)
        {
            var userExist = await _userManager.FindByEmailAsync(adminDto.Email);
            if (userExist != null)
            {
                return "Email address already in use."; // Or a more descriptive error message
            }

            // Create and register new user (including password hashing)

            var newUser = new AppUser
            {
                FullName = adminDto.FullName,
                SecurityStamp = Guid.NewGuid().ToString(),
                Email = adminDto.Email,
                UserName = adminDto.Email

            };

            var role = "Admin";

            if (await _roleManager.RoleExistsAsync(role))
            {
                var result = await _userManager.CreateAsync(newUser, adminDto.Password);

                if (result.Succeeded)
                {

                    //add role to user

                    await _userManager.AddToRoleAsync(newUser, role);


                    //genereate token to verify email 
                    var emailConfirmtoken = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);

                    return emailConfirmtoken;
                }

                else
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new InvalidOperationException($"Failed to register user: {errors}");

                }
            }

            else
            {
                return "Role does not exist";

            }
        }





        //Teacher register 


        //Studnet register 
        public async Task<string> TeacherRegister(TeacherRegisterDto teacherDto)
        {
            var userExist = await _userManager.FindByEmailAsync(teacherDto.Email);
            if (userExist != null)
            {
                return "Email address already in use."; // Or a more descriptive error message
            }

            // Create and register new user (including password hashing)

            var newUser = new AppUser
            {
                UserName = teacherDto.Email,
                FullName = teacherDto.FullName,
                SecurityStamp = Guid.NewGuid().ToString(),
                Email = teacherDto.Email,
                MobileNumber = teacherDto.MobileNumber,
                DateOfBirth = teacherDto.DateOfBirth,
                Gender = teacherDto.Gender,
            };



            var role = "Teacher";

            if (await _roleManager.RoleExistsAsync(role))
            {
                var defaultStudentPassword = "Teacher@1234";
                var result = await _userManager.CreateAsync(newUser, defaultStudentPassword);



                if (result.Succeeded)
                {

                    //add role to user

                    await _userManager.AddToRoleAsync(newUser, role);


                    // Generate registration number
                    var teachersCount = _dbContext.Teachers.Count(); // Get the current count of students
                    var teacherID = GenerateTeacherId(teachersCount);

                    var teacherDetails = new Teacher
                    {
                        AppUserId = newUser.Id, // Assign the newly created user's ID
                        TeacherId = teacherID,

                        HireDate = teacherDto.HireDate,
                        Qualification = teacherDto.Qualification,

                    };

                    _dbContext.Teachers.Add(teacherDetails);
                    await _dbContext.SaveChangesAsync();


                    //genereate token to verify email 
                    var emailConfirmtoken = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);

                    return emailConfirmtoken;
                }

                else
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new InvalidOperationException($"Failed to register user: {errors}");

                }
            }

            else
            {
                return "Role does not eziar";

            }
        }





        //Studnet register 
        public async Task<string> StudentRegister(StudentRegisterDto studentDto)
        {
            var userExist = await _userManager.FindByEmailAsync(studentDto.Email);
            if (userExist != null)
            {
                return "Email address already in use."; // Or a more descriptive error message
            }

            // Create and register new user (including password hashing)

            var newUser = new AppUser
            {
                UserName = studentDto.Email,
                FullName = studentDto.FullName,
                SecurityStamp = Guid.NewGuid().ToString(),
                Email = studentDto.Email,
                MobileNumber = studentDto.MobileNumber,
                DateOfBirth = studentDto.DateOfBirth,
                Gender = studentDto.Gender,
                

            };



            var role = "Student";

            if (await _roleManager.RoleExistsAsync(role))
            {
                var defaultStudentPassword = "Student@1234";
                var result = await _userManager.CreateAsync(newUser, defaultStudentPassword);



                if (result.Succeeded)
                {

                    //add role to user

                    await _userManager.AddToRoleAsync(newUser, role);


                    // Generate registration number
                    var studentCount = _dbContext.Students.Count(); // Get the current count of students
                    var registrationNumber = GenerateStdReg(studentCount);

                    var studentDetails = new Student
                    {
                        AppUserId = newUser.Id, // Assign the newly created user's ID
                        RegistrationNumber = registrationNumber,
                        ClassId=studentDto.ClassId,

                    };

                    _dbContext.Students.Add(studentDetails);
                    await _dbContext.SaveChangesAsync();


                    //genereate token to verify email 
                    var emailConfirmtoken = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);

                    return emailConfirmtoken;
                }

                else
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new InvalidOperationException($"Failed to register user: {errors}");

                }
            }

            else
            {
                return "Role does not eziar";

            }
        }


        // Generate registration number based on the current count of students
        private string GenerateStdReg(int studentCount)
        {
            string prefix = "2024-SRPS-"; // Static prefix
            int paddingLength = 3; // Length of the sequential number part
            string sequentialNumber = (studentCount + 1).ToString().PadLeft(paddingLength, '0'); // Increment the student count and pad with zeros
            return $"{prefix}{sequentialNumber}";
        }

        // Generate Teacher ID based on the current count of Teachers
        private string GenerateTeacherId(int teacherCount)
        {
            string prefix = "SRPS-TCH-"; // Static prefix
            int paddingLength = 3; // Length of the sequential number part
            string sequentialNumber = (teacherCount + 1).ToString().PadLeft(paddingLength, '0'); // Increment the student count and pad with zeros
            return $"{prefix}{sequentialNumber}";
        }



        //login
        public async Task<string> Login(LoginRequestDto loginDto)
        {
            //var userExist = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            var user = await _userManager.FindByEmailAsync(loginDto.Email);


            var passwordMatch = await _userManager.CheckPasswordAsync(user, loginDto.Password);

            if (passwordMatch)
            {
                //await GenerateJwtToken(user);
                var token = await GenerateJwtToken(user);
                return token;
            }
            else
            {
                throw new InvalidOperationException("Password Do not match , please insert correct password");

            }


        }



        //email confirmation password rest link
        public async Task SendEmailAsync(string emailAddress, string subject, string content)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse("no-reply@team-usama-suhaib"));
                email.To.Add(MailboxAddress.Parse(emailAddress));
                email.Subject = subject;
                //email.Body = new TextPart(TextFormat.Html) { Text = content };

                email.Body = new TextPart(TextFormat.Html)
                {
                    Text = content
                };
                using (var smtp = new SmtpClient())
                {
                    smtp.Connect(_configuration.GetSection("EmailHost").Value, 587, SecureSocketOptions.StartTls);
                    smtp.Authenticate(_configuration.GetSection("EmailUsername").Value, _configuration.GetSection("EmailPassword").Value);
                    await smtp.SendAsync(email);
                    smtp.Disconnect(true);
                }
            }
            catch (Exception ex)
            {
                // Log the error or handle it appropriately
                throw new ApplicationException("Failed to send email", ex);
            }
        }




        public async Task<string> GenerateJwtToken(AppUser user)
        {

            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtConfig.Secret);

            // Get user roles from UserManager
            var roles = await _userManager.GetRolesAsync(user);


            // Define claims for the JWT token
            var claims = new List<Claim>
            {
                //new Claim("Id", user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Name, user.FullName),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToUniversalTime().ToString())
            };

            // Add user roles as claims (if any)
            if (roles != null && roles.Any())
            {
                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
            }



            // Create a token descriptor with the defined claims
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };



            // Generate and serialize the JWT token
            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = jwtTokenHandler.WriteToken(token);

            return jwtToken;
        }



    }
}
