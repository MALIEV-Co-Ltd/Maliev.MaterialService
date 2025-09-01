using Maliev.MaterialService.Api.DTOs;
using Maliev.MaterialService.Api.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Asp.Versioning;

namespace Maliev.MaterialService.Api.Controllers
{
    [ApiController]
    [Route("v{version:apiVersion}/materials")]
    [ApiVersion("1.0")]
    public class MaterialsController : ControllerBase
    {
        private readonly IMaterialServiceService _materialService;

        public MaterialsController(IMaterialServiceService materialService)
        {
            _materialService = materialService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MaterialDto>>> GetMaterials()
        {
            var materials = await _materialService.GetAllMaterialsAsync();
            return Ok(materials);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MaterialDto>> GetMaterial(int id)
        {
            var material = await _materialService.GetMaterialByIdAsync(id);
            if (material == null)
            {
                return NotFound();
            }
            return Ok(material);
        }

        [HttpPost]
        public async Task<ActionResult<MaterialDto>> CreateMaterial(CreateMaterialRequest request)
        {
            var material = await _materialService.CreateMaterialAsync(request);
            return CreatedAtAction(nameof(GetMaterial), new { id = material.Id }, material);
        }

        

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMaterial(int id)
        {
            var result = await _materialService.DeleteMaterialAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}