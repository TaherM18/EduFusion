// using Repositories.Interfaces;
// using Repositories.Models;
// using Microsoft.AspNetCore.Mvc;
// using System.Collections.Generic;
// using System.Threading.Tasks;

// namespace API.Controller
// {
//     [Route("api/materials")]
//     [ApiController]
//     public class MaterialApiController : ControllerBase
//     {
//         private readonly IMaterialInterface _materialRepository;

//         public MaterialApiController(IMaterialInterface materialRepository)
//         {
//             _materialRepository = materialRepository;
//         }

//         [HttpGet]
//         [Route("GetAllMaterials")]
//         public async Task<ActionResult<IEnumerable<Material>>> GetAllMaterials()
//         {
//             var materials = await _materialRepository.GetAllMaterialsAsync();
//             return Ok(materials);
//         }

//         [HttpGet]
//         [Route("GetMaterialById/{id}")]
//         public async Task<ActionResult> GetMaterialById(int id)
//         {
//             var material = await _materialRepository.GetMaterialByIdAsync(id);
//             if (material == null)
//                 return NotFound();

//             return Ok(material);
//         }

//         [HttpPost]
//         [Route("AddMaterial")]
//         public async Task<IActionResult> AddMaterial([FromForm] Material materialDto)
//         {
//             if (materialDto == null || materialDto.File == null)
//                 return BadRequest("Invalid material data.");

//             var material = new Material
//             {
//                 FileName = materialDto.File.FileName,
//                 UserID = materialDto.UserID, // Ensure UserID is passed correctly
//                 SubjectID = materialDto.SubjectID
//             };

//             int newMaterialId = await _materialRepository.AddMaterialAsync(material);

//             if (newMaterialId > 0)
//             {
//                 return Ok(new { message = "Material added successfully", id = newMaterialId });
//             }

//             return StatusCode(500, "Failed to add material.");
//         }

//         [HttpDelete]
//         [Route("DeleteMaterial/{id}")]
//         public async Task<IActionResult> DeleteMaterial(int id)
//         {
//             var isDeleted = await _materialRepository.DeleteMaterialAsync(id);
//             if (!isDeleted)
//                 return NotFound();

//             return NoContent();
//         }
//     }
// }
