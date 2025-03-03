using Repositories.Models;
using Repositories.Interfaces;

using Microsoft.AspNetCore.Mvc;

namespace MVC.Controllers
{
    [ApiController]
    [Route("api/subject-tracking")]
    public class SubjectTrackingApiController : ControllerBase
    {
        private readonly ISubjectTrackingInterface _subjectTrackingRepository;

        public SubjectTrackingApiController(ISubjectTrackingInterface subjectTrackingRepository)
        {
            _subjectTrackingRepository = subjectTrackingRepository;
        }

        #region GetAll
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _subjectTrackingRepository.GetAll();

            if (result == null)
                return StatusCode(500, new { message = "There was some error while fetching subject tackings" });

            return Ok(result);
        }
        #endregion


        #region GetOne
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOne(int id)
        {
            var result = await _subjectTrackingRepository.GetOne(id);

            if (result == null)
                return NotFound();

            return Ok(result);
        }
        #endregion


        #region GetAllByStandard
        [HttpGet("standard/{id}")]
        public async Task<IActionResult> GetAllByStandard(int id)
        {
            var result = await _subjectTrackingRepository.GetAllByStandard(id);

            if (result == null)
                return StatusCode(500, new { message = "There was some error while fetching subject tackings for standard" });

            return Ok(result);
        }
        #endregion


        #region GetAllByTeacher
        [HttpGet("teacher/{id}")]
        public async Task<IActionResult> GetAllByTeacher(int id)
        {
            var result = await _subjectTrackingRepository.GetAllByTeacher(id);

            if (result == null)
                return StatusCode(500, new { message = "There was some error while fetching subject tackings for teacher" });

            return Ok(result);
        }
        #endregion


        #region Add
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] SubjectTracking data)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _subjectTrackingRepository.Add(data);

            if (result == -1)
                return StatusCode(500, new { message = "There was some error while adding subject tracking" });

            return result > 0 
                ? Ok(new { message = "Added successfully" })
                : BadRequest("Failed to add");
        }
        #endregion


        #region Update
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] SubjectTracking data)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _subjectTrackingRepository.Update(data);

            if (result == -1)
                return StatusCode(500, new { message = "There was some error while updating subject tracking" });

            return result > 0 
                ? Ok(new { message = "Updated successfully" }) 
                : BadRequest("Failed to update");
        }
        #endregion


        #region Delete
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _subjectTrackingRepository.Delete(id);

            return result > 0 
                ? Ok(new { message = "Deleted successfully" }) 
                : BadRequest("Failed to delete");
        }
        #endregion
    }
}