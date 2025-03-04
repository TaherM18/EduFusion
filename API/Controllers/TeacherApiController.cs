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
            _profileImagePath = "../MVC/wwwroot/profile_images";
            _fileHelper = new FileHelper();
        }

        #region Register
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromForm] Teacher teacher)
        {
            try
            {
                if (teacher.User?.ImageFile != null)
                {
                    teacher.User.Image = await _fileHelper.UploadProfileImage(_profileImagePath, teacher.User.ImageFile, teacher.User?.Image);
                }
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
        [HttpPut]
        public async Task<IActionResult> Update([FromForm] Teacher teacher)
        {
            try
            {
                if (teacher == null)
                    return BadRequest(new { message = "Invalid request data" });

                if (teacher.User?.ImageFile != null)
                {
                    teacher.User.Image = await _fileHelper.UploadProfileImage(_profileImagePath, teacher.User.ImageFile, teacher.User?.Image);
                }

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

        #region Student Progress
        [HttpGet]
        [Route("GetStudentProgress")]
        public async Task<IActionResult> GetStudentProgress()
        {
            try
            {
                List<StudentProgress> result = await _teacherRepo.GetStudentProgress();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region Student Ratings
        [HttpGet]
        [Route("GetStudentRatings")]
        public async Task<IActionResult> GetStudentRatings()
        {
            try
            {
                List<StudentRating> result = await _teacherRepo.GetStudentRatings();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region Student Standards
        [HttpGet]
        [Route("GetStudentStandards")]
        public async Task<IActionResult> GetStudentStandards()
        {
            try
            {
                List<Standard> result = await _teacherRepo.GetStandards();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region Student Subjects
        [HttpGet]
        [Route("GetStudentSubjects/{id}")]
        public async Task<IActionResult> GetStudentSubjects(int id)
        {
            try
            {
                List<Subject> result = await _teacherRepo.GetSubjects(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region Get Teacher By Subject
        [HttpGet]
        [Route("GetTeacherBySubject/{id}")]
        public async Task<IActionResult> GetTeacherBySubject(int id)
        {
            try
            {
                Teacher result = await _teacherRepo.GetTeacherBySubject(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region Get TimeTable
        [HttpGet]
        [Route("GetTimeTable")]
        public async Task<IActionResult> GetTimeTable()
        {
            try
            {
                List<TimeTable> result = await _teacherRepo.GetTimeTable();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region Add Schedule
        [HttpPost]
        [Route("AddSchedule")]
        public async Task<IActionResult> AddSchedule([FromForm]TimeTable timeTable)
        {
            try
            {
                int result = await _teacherRepo.AddTimeTable(timeTable);
                if (result > 0)
                {
                    return Ok("Schedule Added Successfully");
                }
                else
                {
                    return BadRequest("Failed to Add Schedule");
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