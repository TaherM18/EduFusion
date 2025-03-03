using Helpers.Files;
using Microsoft.AspNetCore.Mvc;
using Repositories.Interfaces;
using Repositories.Models;

namespace API.Controllers
{
    [Route("api/materials")]
    [ApiController]
    public class MaterialApiController : ControllerBase
    {
        private readonly IMaterialInterface _materialRepository;
        private readonly FileHelper _fileHelper;
        private readonly string _materialDirectoryPath = "../MVC/wwwroot/materials";

        public MaterialApiController(IMaterialInterface materialRepository)
        {
            _materialRepository = materialRepository;
            _fileHelper = new FileHelper();
        }
        
        
        #region GetAllAsync
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Material>>> GetAllAsync()
        {
            var materials = await _materialRepository.GetAll();
            return Ok(materials);
        }
        #endregion


        #region GetOneAsync
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult> GetOneAsync(int id)
        {
            var material = await _materialRepository.GetOne(id);
            if (material == null)
                return NotFound();

            return Ok(material);
        }
        #endregion


        #region AddAsync
        [HttpPost]
        public async Task<IActionResult> AddAsync([FromForm] Material material)
        {
            if (material == null || material.MaterialFile == null)
                return BadRequest("Invalid material data.");

            material.FileName = await _fileHelper.UploadFile(_materialDirectoryPath, material.MaterialFile, material.FileName);

            int newMaterialId = await _materialRepository.Add(material);

            if (newMaterialId > 0)
            {
                return Ok(new { message = "Material added successfully", id = newMaterialId });
            }

            return StatusCode(500, "Failed to add material.");
        }
        #endregion


        #region DeleteAsync
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            Material material = await _materialRepository.GetOne(id);

            if (!string.IsNullOrEmpty(material.FileName))
            {
                await _fileHelper.DeleteFile(Path.Combine(_materialDirectoryPath, material.FileName));
            }

            var result = await _materialRepository.Delete(id);

            if (result <= 0)
                return NotFound(new { message = "No material found" });

            return NoContent();
        }
        #endregion


        #region Download
        [HttpGet("download/{id}")]
        public async Task<IActionResult> Download(int id)
        {
            var material = await _materialRepository.GetOne(id);

            if (material == null || string.IsNullOrEmpty(material.FileName))
                return NotFound();

            string filePath = Path.Combine(_materialDirectoryPath, material.FileName);

            if (!System.IO.File.Exists(filePath))
                return NotFound();

            var bytes = await System.IO.File.ReadAllBytesAsync(filePath);

            return File(bytes, "application/octet-stream", material.FileName);
        }
        #endregion
    }
}