using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Repositories.Models;
using Repositories.Interfaces;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserInterface _user;
        private readonly IConfiguration _config;

        public UserController(IConfiguration configuration, IUserInterface userInterface)
        {
            _config = configuration;
            _user = userInterface;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromForm] LoginVM user)
        {
            User UserData = await _user.Login(user);
            if (UserData.UserID != 0)
            {
                Console.WriteLine($"UserID :: {UserData.UserID} :: {UserData.Role}");
                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("Userid", UserData.UserID.ToString()),
                    new Claim("UserName", UserData.FirstName),
                    new Claim(ClaimTypes.Role, UserData.Role)
                };
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
                var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: signIn
                );

                if (UserData.Role == "S")
                {
                    Student s = await _user.GetStudent(UserData, UserData.UserID ?? 0);
                    System.Console.WriteLine("Student is approved :: ", s.IsApproved);
                    if (s.IsApproved == true)
                    {
                        return Ok(new
                        {
                            success = true,
                            message = "Login Success",
                            UserData = await _user.GetStudent(UserData, UserData.UserID ?? 0),
                            token = new JwtSecurityTokenHandler().WriteToken(token)
                        });
                    }
                    else
                    {
                        return BadRequest(new
                        {
                            success = false,
                            message = "User is not approved yet",
                            UserData = await _user.GetStudent(UserData, UserData.UserID ?? 0),
                            token = new JwtSecurityTokenHandler().WriteToken(token)
                        });
                    }
                }
                if (UserData.Role == "T")
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Login Success",
                        UserData = await _user.GetTeacher(UserData, UserData.UserID ?? 0),
                        token = new JwtSecurityTokenHandler().WriteToken(token)
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Login Success",
                    UserData = UserData,
                    token = new JwtSecurityTokenHandler().WriteToken(token)
                });
            }
            return BadRequest(new
            {
                message = "Login unsuccessfull"
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUsers(int id)
        {
            List<User> users = new List<User>();
            if (await _user.GetAdmin(id) == 1)
            {
                users = await _user.GetUsers();
                return Ok(users);
            }
            else return BadRequest(new
            {
                success = "false",
                message = "Only admin can call this api."
            });

        }

    }
}