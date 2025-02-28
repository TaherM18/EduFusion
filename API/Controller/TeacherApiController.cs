using Microsoft.AspNetCore.Mvc;
using EduFusion.Repositories.Models;
using EduFusion.Repositories.Interfaces;

namespace API.Controller
{
    [ApiController]
    [Route("api/teacher")]
    public class TeacherApiController : ControllerBase
    {
        private readonly IStudentInterface _studRepo;

        public TeacherApiController(IStudentInterface student)
        {
            _studRepo = student;
        }
        [HttpPost]
        public async Task<IActionResult> Register([FromForm] Teacher teacher)
        {
            try
            {
                int studId = await _studRepo.TeacherRegister(teacher);
                return Ok(new
                {
                    success = true,
                    message = "Registration Successful!",
                    data = studId
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}