using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EduFusion.Repositories.Models;
using EduFusion.Repositories.Interfaces;


namespace EduFusion.API.Controller
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

        [HttpPost]
        public async Task<IActionResult> Register([FromForm] Student student)
        {
            try
            {
                int studId = await _studRepo.Register(student);
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