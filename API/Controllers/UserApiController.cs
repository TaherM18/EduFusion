using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Repositories.Interfaces;
using Repositories.Models;

namespace API.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserApiController : ControllerBase
    {
        private readonly IUserInterface _user;
        private readonly IConfiguration _config;

        public UserApiController(IConfiguration configuration, IUserInterface userInterface)
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
                    return Ok(new
                    {
                        success = true,
                        message = "Login Success",
                        UserData = await _user.GetStudent(UserData, UserData.UserID ?? 0),
                        token = new JwtSecurityTokenHandler().WriteToken(token)
                    });
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
    }
}