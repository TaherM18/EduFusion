using Microsoft.AspNetCore.Mvc;
using Repositories.Models;
using Repositories.Interfaces;


namespace API.Controller
{
    [ApiController]
    [Route("api/student")]
    public class StudentApiController : ControllerBase
    {
        private readonly IStudentInterface _studRepo;

        public StudentApiController(IStudentInterface student)
        {
            _studRepo = student;
        }

        #region Register
        [HttpPost]
        public async Task<IActionResult> Register([FromForm] Student student)
        {
            try
            {
                int userId = await _studRepo.Register(student);
                if (userId != 0) {
                    return Ok(new
                    {
                        success = true,
                        message = "Registration Successful!",
                        data = userId
                    });
                }
                else {
                    return BadRequest(new
                    {
                        success = false,
                        message = "There was some error while performing Registration.",
                        data = userId
                    });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        
    }
}