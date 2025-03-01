using Microsoft.AspNetCore.Mvc;
using Repositories.Models;
using Repositories.Interfaces;

namespace API.Controller
{
    [ApiController]
    [Route("api/teacher")]
    public class TeacherApiController : ControllerBase
    {
        private readonly ITeacherInterface _teacherRepo;

        public TeacherApiController(ITeacherInterface teacher)
        {
            _teacherRepo = teacher;
        }
        [HttpPost]
        public async Task<IActionResult> Register([FromForm] Teacher teacher)
        {
            try
            {
                int userId = await _teacherRepo.Register(teacher);
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
    }
}