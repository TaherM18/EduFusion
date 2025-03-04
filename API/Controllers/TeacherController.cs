// using Microsoft.AspNetCore.Mvc;
// using Repositories.Interfaces;
// using Repositories.Models;

// namespace API.Controller
// {
//     [ApiController]
//     [Route("api/[controller]")]
//     public class TeacherController : ControllerBase
//     {
//         public ITeacherInterface _teacher;
//         public TeacherController(ITeacherInterface teacherInterface)
//         {
//             _teacher = teacherInterface;
//         }

//         #region Student Progress
//         [HttpGet]
//         [Route("GetStudentProgress")]
//         public async Task<IActionResult> GetStudentProgress()
//         {
//             try
//             {
                
//                 List<StudentProgress> result = await _teacher.GetStudentProgress();
//                 return Ok(result);
//             }
//             catch (Exception ex)
//             {
//                 return BadRequest(ex.Message);
//             }
//         }
//         #endregion

//         #region Student Ratings
//         [HttpGet]
//         [Route("GetStudentRatings")]
//         public async Task<IActionResult> GetStudentRatings()
//         {
//             try
//             {
//                 List<StudentRating> result = await _teacher.GetStudentRatings();
//                 return Ok(result);
//             }
//             catch (Exception ex)
//             {
//                 return BadRequest(ex.Message);
//             }
//         }
//         #endregion

//         #region Student Standards
//         [HttpGet]
//         [Route("GetStudentStandards")]
//         public async Task<IActionResult> GetStudentStandards()
//         {
//             try
//             {
//                 List<Standard> result = await _teacher.GetStandards();
//                 return Ok(result);
//             }
//             catch (Exception ex)
//             {
//                 return BadRequest(ex.Message);
//             }
//         }
//         #endregion

//         #region Student Subjects
//         [HttpGet]
//         [Route("GetStudentSubjects/{id}")]
//         public async Task<IActionResult> GetStudentSubjects(int id)
//         {
//             try
//             {
//                 List<Subject> result = await _teacher.GetSubjects(id);
//                 return Ok(result);
//             }
//             catch (Exception ex)
//             {
//                 return BadRequest(ex.Message);
//             }
//         }
//         #endregion

//         #region Get Teacher By Subject
//         [HttpGet]
//         [Route("GetTeacherBySubject/{id}")]
//         public async Task<IActionResult> GetTeacherBySubject(int id)
//         {
//             try
//             {
//                 Teacher result = await _teacher.GetTeacherBySubject(id);
//                 return Ok(result);
//             }
//             catch (Exception ex)
//             {
//                 return BadRequest(ex.Message);
//             }
//         }
//         #endregion

//     }
// }