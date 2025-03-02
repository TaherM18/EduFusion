using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Repositories.Interfaces;
using Repositories.Models;

namespace API.Controllers
{
    [ApiController]
    [Route("api/exam")]
    public class ExamApiController : ControllerBase
    {
        private readonly IExamInterface _examRepository;

        public ExamApiController(IExamInterface examRepository)
        {
            _examRepository = examRepository;
        }

        #region GetAll
        // GET: api/exams
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var exams = await _examRepository.GetAll();
            if (exams == null)
                return NotFound(new { message = "No exams found." });

            return Ok(exams);
        }
        #endregion


        #region GetOne
        // GET: api/exams/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOne(int id)
        {
            var exam = await _examRepository.GetOne(id);
            if (exam == null)
                return NotFound(new { message = $"Exam with ID {id} not found." });

            return Ok(exam);
        }
        #endregion


        #region GetAllByStandard
        // GET: api/exams/standard/{standardID}
        [HttpGet("standard/{id}")]
        public async Task<IActionResult> GetAllByStandard(int id)
        {
            var exams = await _examRepository.GetAllByStandard(id);
            if (exams == null)
                return NotFound(new { message = $"No exams found for standard ID {id}." });

            return Ok(exams);
        }
        #endregion


        #region Add
        // POST: api/exams
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Exam exam)
        {
            if (exam == null)
                return BadRequest(new { message = "Invalid exam data." });

            var examId = await _examRepository.Add(exam);
            if (examId <= 0)
                return StatusCode(500, new { message = "Failed to add exam." });

            return CreatedAtAction(nameof(GetOne), new { id = examId }, new { message = "Exam added successfully.", examId });
        }
        #endregion


        #region Update
        // PUT: api/exams/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Exam exam)
        {
            if (exam == null || id != exam.ExamID)
                return BadRequest(new { message = "Invalid request data." });

            var updatedRows = await _examRepository.Update(exam);
            if (updatedRows <= 0)
                return StatusCode(500, new { message = $"Failed to update exam with ID {id}." });

            return Ok(new { message = "Exam updated successfully." });
        }
        #endregion


        #region Delete
        // DELETE: api/exams/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deletedRows = await _examRepository.Delete(id);
            if (deletedRows <= 0)
                return StatusCode(500, new { message = $"Failed to delete exam with ID {id}." });

            return Ok(new { message = "Exam deleted successfully." });
        }
        #endregion
    }

}