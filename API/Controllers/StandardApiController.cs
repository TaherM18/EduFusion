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
    [Route("api/standard")]
    public class StandardApiController : ControllerBase
    {
        private readonly IStandardInterface _standardRepository;

        public StandardApiController(IStandardInterface standardRepository)
        {
            _standardRepository = standardRepository;
        }

        #region GetAll
        // GET: api/standard
        [HttpGet]
        public async Task<ActionResult<List<Standard>>> GetAll()
        {
            var standards = await _standardRepository.GetAll();

            if (standards == null)
                return StatusCode(500, new { message = "There was some error while fetching standards" });

            return Ok(standards);
        }
        #endregion


        #region GetOne
        // GET: api/standard/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Standard>> GetOne(int id)
        {
            var standard = await _standardRepository.GetOne(id);

            if (standard == null)
                return NotFound(new { message = "Standard not found" });

            return Ok(standard);
        }
        #endregion


        #region Add
        // POST: api/standard
        [HttpPost]
        public async Task<ActionResult<int>> Add([FromBody] Standard standard)
        {
            if (standard == null || string.IsNullOrWhiteSpace(standard.StandardName))
            {
                return BadRequest(new { message = "Invalid data provided" });
            }

            int newId = await _standardRepository.Add(standard);

            return CreatedAtAction(nameof(GetOne), new { id = newId },
                new { message = "Standard added successfully", id = newId }
            );
        }
        #endregion


        #region Update
        // PUT: api/standard/{id}
        [HttpPut]
        public async Task<ActionResult> Update([FromBody] Standard standard)
        {
            if (standard == null)
            {
                return BadRequest(new { message = "Invalid data or ID mismatch" });
            }

            int rowsAffected = await _standardRepository.Update(standard);
            if (rowsAffected == 0)
            {
                return NotFound(new { message = "Standard not found or not updated" });
            }

            return NoContent();
        }
        #endregion


        #region Delete
        // DELETE: api/standard/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            int rowsAffected = await _standardRepository.Delete(id);
            if (rowsAffected == 0)
            {
                return NotFound(new { message = "Standard not found or already deleted" });
            }

            return NoContent();
        }
        #endregion
    }
}