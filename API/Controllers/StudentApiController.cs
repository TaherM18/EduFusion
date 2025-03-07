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

        public StudentApiController(IStudentInterface student)
        {
            _profileImagePath = "../MVC/wwwroot/profile_images";
            _studRepo = student;
            _fileHelper = new FileHelper();
        }

        #region Register
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromForm] StudentViewModel student)
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
        // GET: api/student
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Student>>> GetAllStudents()
        {
            var students = await _studRepo.GetAll();
            return Ok(students);
        }
        #endregion

        #region Approve
        // GET: api/student
        [HttpPut("approve/{id}")]
        public async Task<ActionResult<IEnumerable<Student>>> ApproveStudent(int id)
        {
            int result = await _studRepo.Approve(id);
            if (result == 1)
            {
                return Ok(new
                {
                    message = "Student Aprroved successfully",
                    success = "true"
                });
            }
            else
            {
                return BadRequest(new
                {
                    message = "Something went wrong",
                    success = "false"
                });
            }
        }
        #endregion

        #region UnApprove
        // GET: api/student
        [HttpPut("unapprove/{id}")]
        public async Task<ActionResult<IEnumerable<Student>>> UnApproveStudent(int id)
        {
            int result = await _studRepo.UnApprove(id);
            if (result == 1)
            {
                return Ok(new
                {
                    message = "Student UnAprroved successfully",
                    success = "true"
                });
            }
            else
            {
                return BadRequest(new
                {
                    message = "Something went wrong",
                    success = "false"
                });
            }
        }
        #endregion


        #region Get
        // GET: api/student/{id}
        [HttpGet("{id:int}")]
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
        // POST: api/student
        [HttpPost]
        public async Task<ActionResult<int>> CreateStudent([FromForm] Student student)
        {
            if (student == null)
            {
                return BadRequest(new { message = "Invalid student data" });
            }

            if (student.User?.ImageFile != null)
            {
                student.User.Image = await _fileHelper.UploadFile(_profileImagePath, student.User.ImageFile, student.User?.Image);
            }

            var studentId = await _studRepo.Add(student);

            if (studentId <= 0)
            {
                return StatusCode(500, new { message = "Failed to add student" });
            }

            return CreatedAtAction(nameof(GetStudent), new { id = studentId }, new { message = "Student added successfully", id = studentId });
        }
        #endregion


        #region Update
        // PUT: api/student/{id}
        [HttpPut]
        public async Task<IActionResult> UpdateStudent([FromForm] Student student)
        {
            if (student == null)
            {
                return BadRequest(new { message = "Invalid request data" });
            }

            if (student.User?.ImageFile != null)
            {
                student.User.Image = await _fileHelper.UploadFile(_profileImagePath, student.User.ImageFile, student.User?.Image);
            }


            // Exclude password and other unnecessary fields during update
            if (student.User != null)
            {
                student.User.Password = null; // Prevents validation errors on Password
            }

            var updated = await _studRepo.Update(student);
            if (updated <= 0)
            {
                return NotFound(new { message = "Student not found or update failed" });
            }

            return NoContent(); // 204 No Content
        }
        #endregion


        #region UpdatewithVM
        // PUT: api/student/update
        [HttpPut]
        [Route("update")]
        public async Task<IActionResult> UpdateStudentVM([FromForm] StudentViewModel student)
        {
            if (student == null)
            {
                return BadRequest(new { message = "Invalid request data" });
            }

            if (student?.ImageFile != null)
            {
                student.Image = await _fileHelper.UploadFile(_profileImagePath, student.ImageFile, student?.Image);
            }

            student.Password = null; // Prevents validation errors on Password

            var updated = await _studRepo.Update(student);
            if (updated <= 0)
            {
                return NotFound(new { message = "Student not found or update failed" });
            }

            return NoContent(); // 204 No Content
        }
        #endregion


        #region Delete
        // DELETE (Soft Delete): api/student/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDeleteStudent(int id)
        {
            var deleted = await _studRepo.Delete(id);
            if (deleted >= 0)
            {
                return Ok(new { message = "Successfully deleted", success = "true" });
            }

            return NoContent(); // 204 No Content
        }
        #endregion
    }
}
