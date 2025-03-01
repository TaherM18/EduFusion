using Microsoft.AspNetCore.Mvc;
using Repositories.Models;
using Repositories.Interfaces;
using Helpers.Files;

namespace API.Controller
{
    [ApiController]
    [Route("api/teacher")]
    public class TeacherApiController : ControllerBase
    {
        private readonly ITeacherInterface _teacherRepo;
        private readonly FileHelper _fileHelper;
        private readonly string _profileImagePath;

        public TeacherApiController(ITeacherInterface teacher, IWebHostEnvironment env)
        {
            _teacherRepo = teacher;
            _profileImagePath = Path.Combine(env.WebRootPath, "profile_images");
            _fileHelper = new FileHelper();
        }

        #region Register
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromForm] Teacher teacher)
        {
            try
            {
                int userId = await _teacherRepo.Register(teacher);
                if (userId != 0)
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Registration Successful!",
                        data = userId
                    });
                }
                else
                {
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


        #region Get One
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOne(int id)
        {
            var teacher = await _teacherRepo.GetOne(id);
            if (teacher == null) return NotFound(new { message = "Teacher not found" });

            return Ok(teacher);
        }
        #endregion

        #region Get All
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var teachers = await _teacherRepo.GetAll();
            return Ok(teachers);
        }
        #endregion

        #region Add
        [HttpPost]
        public async Task<IActionResult> Add([FromForm] Teacher teacher)
        {
            try
            {
                if (teacher.User?.ImageFile != null)
                {
                    teacher.User.Image = await _fileHelper.UploadProfileImage(_profileImagePath, teacher.User.ImageFile, teacher.User.Image);
                }

                int result = await _teacherRepo.Add(teacher);
                if (result > 0)
                    return CreatedAtAction(nameof(GetOne), new { id = result }, new { message = "Teacher added successfully", id = result });

                return BadRequest(new { message = "Failed to add teacher" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal Server Error", error = ex.Message });
            }
        }
        #endregion

        #region Update
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] Teacher teacher)
        {
            try
            {
                var existingTeacher = await _teacherRepo.GetOne(id);
                if (existingTeacher == null)
                    return NotFound(new { message = "Teacher not found" });

                if (teacher.User?.ImageFile != null)
                {
                    teacher.User.Image = await _fileHelper.UploadProfileImage(_profileImagePath, teacher.User.ImageFile, existingTeacher.User?.Image);
                }

                teacher.User.UserID = id;
                int result = await _teacherRepo.Update(teacher);

                if (result > 0)
                    return Ok(new { message = "Teacher updated successfully" });

                return BadRequest(new { message = "Failed to update teacher" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal Server Error", error = ex.Message });
            }
        }
        #endregion

        #region Soft Delete
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var teacher = await _teacherRepo.GetOne(id);
                if (teacher == null)
                    return NotFound(new { message = "Teacher not found" });

                int result = await _teacherRepo.Delete(id);
                if (result > 0)
                    return Ok(new { message = "Teacher soft deleted successfully" });

                return BadRequest(new { message = "Failed to delete teacher" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal Server Error", error = ex.Message });
            }
        }
        #endregion
    }
}