// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using Microsoft.AspNetCore.Mvc;
// using Repositories.Implementations;
// using Repositories.Interfaces;
// using Repositories.Models;

// namespace API.Controllers
// {
//     [ApiController]
//     [Route("api/timetable")]
//     public class TimeTableApiController : ControllerBase
//     {
//         private readonly ITimeTableInterface _timeTableRepo;

//         public TimeTableApiController(ITimeTableInterface timeTableRepo)
//         {
//             _timeTableRepo = timeTableRepo;
//         }

//         #region GetAllGrouped
//         // Get all timetables grouped by day for a standard
//         [HttpGet("standard/{id:int}")]
//         public async Task<ActionResult<Dictionary<string, List<TimeTable>>>> GetAllGrouped(int id)
//         {
//             var result = await _timeTableRepo.GetAllByStandardGroupByDayOfWeek(id);
//             if (result == null)
//             {
//                 return StatusCode(500, new {
//                     message = "There was some error while retrieving timetable."
//                 });
//             }
//             return Ok(result);
//         }
//         #endregion


//         #region GetOne
//         [HttpGet("{id}")]
//         public async Task<ActionResult<TimeTable>> GetOne(int id)
//         {
//             var timetable = await _timeTableRepo.GetOne(id);
//             if (timetable == null)
//                 return NotFound(new { message = "Timetable entry not found" });

//             return Ok(timetable);
//         }
//         #endregion


//         #region Add
//         [HttpPost]
//         public async Task<ActionResult<int>> Add([FromBody] TimeTable data)
//         {
//             if (data == null)
//                 return BadRequest(new { message = "Invalid data" });

//             int newId = await _timeTableRepo.Add(data);
//             return CreatedAtAction(nameof(GetOne), new { id = newId }, new { timetableID = newId });
//         }
//         #endregion



//         #region Update
//         [HttpPut("{id}")]
//         public async Task<ActionResult> Update(int id, [FromBody] TimeTable data)
//         {
//             if (data == null || id != data.TimetableID)
//                 return BadRequest(new { message = "Invalid data or ID mismatch" });

//             int rowsAffected = await _timeTableRepo.Update(data);
//             if (rowsAffected == 0)
//                 return NotFound(new { message = "Timetable entry not found" });

//             return NoContent();
//         }
//         #endregion
        

//         #region Delete
//         [HttpDelete("{id}")]
//         public async Task<ActionResult> Delete(int id)
//         {
//             int rowsAffected = await _timeTableRepo.Delete(id);
//             if (rowsAffected == 0)
//                 return NotFound(new { message = "Timetable entry not found" });

//             return NoContent();
//         }
//         #endregion

//     }
// }