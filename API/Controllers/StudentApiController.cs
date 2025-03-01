using Microsoft.AspNetCore.Mvc;
using Repositories.Models;
using Repositories.Interfaces;
using Helpers.Files;


namespace API.Controller
{
    [Route("api/student")]
    [ApiController]
    public class StudentApiController : ControllerBase
    {
        private readonly IStudentInterface _studRepo;
        private readonly FileHelper _fileHelper;
        private readonly string _profileImagePath;

        public StudentApiController(IStudentInterface student, IWebHostEnvironment env)
        {
            _profileImagePath = Path.Combine(env.WebRootPath, "profile_images");
            _studRepo = student;
            _fileHelper = new FileHelper();
        }

        #region Register
        [HttpPost("/Register")]
        public async Task<IActionResult> Register([FromForm] Student student)
        {
            try
            {
                int userId = await _studRepo.Register(student);
                if (userId > 0)
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


        #region GetAll
        // GET: api/students
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Student>>> GetAllStudents()
        {
            var students = await _studRepo.GetAll();
            return Ok(students);
        }
        #endregion


        #region Get
        // GET: api/students/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Student>> GetStudent(int id)
        {
            var student = await _studRepo.GetOne(id);
            if (student == null)
            {
                return NotFound(new { message = "Student not found" });
            }
            return Ok(student);
        }
        #endregion


        #region Create
        // POST: api/students
        [HttpPost]
        public async Task<ActionResult<int>> CreateStudent([FromBody] Student student)
        {
            if (student == null)
            {
                return BadRequest(new { message = "Invalid student data" });
            }

            string? fileName = await _fileHelper.UploadProfileImage(_profileImagePath, student.User.ImageFile, student.User.Image);
            student.User.Image = fileName;

            var studentId = await _studRepo.Add(student);

            if (studentId <= 0)
            {
                return StatusCode(500, new { message = "Failed to add student" });
            }

            return CreatedAtAction(nameof(GetStudent), new { id = studentId }, student);
        }
        #endregion


        #region Update
        // PUT: api/students/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudent(int id, [FromBody] Student student)
        {
            if (student == null || id != student.StudentID)
            {
                return BadRequest(new { message = "Invalid request data" });
            }

            string? fileName = await _fileHelper.UploadProfileImage(_profileImagePath, student.User.ImageFile, student.User.Image);
            student.User.Image = fileName;
            
            var updated = await _studRepo.Update(student);
            if (updated <= 0)
            {
                return NotFound(new { message = "Student not found or update failed" });
            }

            return NoContent(); // 204 No Content
        }
        #endregion


        #region Delete
        // DELETE (Soft Delete): api/students/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDeleteStudent(int id)
        {
            var deleted = await _studRepo.Delete(id);
            if (deleted <= 0)
            {
                return NotFound(new { message = "Student not found or already inactive" });
            }

            return NoContent(); // 204 No Content
        }
        #endregion
    }
}
