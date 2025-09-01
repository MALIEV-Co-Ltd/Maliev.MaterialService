using Maliev.MaterialService.Api.DTOs;
using Maliev.MaterialService.Api.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Asp.Versioning;

namespace Maliev.MaterialService.Api.Controllers
{
    [ApiController]
    [Route("v{version:apiVersion}/materialgroups")]
    [ApiVersion("1.0")]
    public class MaterialGroupsController : ControllerBase
    {
        private readonly IMaterialServiceService _materialService;

        public MaterialGroupsController(IMaterialServiceService materialService)
        {
            _materialService = materialService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MaterialGroupDto>>> GetMaterialGroups()
        {
            var materialGroups = await _materialService.GetAllMaterialGroupsAsync();
            return Ok(materialGroups);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MaterialGroupDto>> GetMaterialGroup(int id)
        {
            var materialGroup = await _materialService.GetMaterialGroupByIdAsync(id);
            if (materialGroup == null)
            {
                return NotFound();
            }
            return Ok(materialGroup);
        }

        [HttpPost]
        public async Task<ActionResult<MaterialGroupDto>> CreateMaterialGroup(CreateMaterialGroupRequest request)
        {
            var materialGroup = await _materialService.CreateMaterialGroupAsync(request);
            return CreatedAtAction(nameof(GetMaterialGroup), new { id = materialGroup.Id }, materialGroup);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<MaterialGroupDto>> UpdateMaterialGroup(int id, UpdateMaterialGroupRequest request)
        {
            if (id != request.Id)
            {
                return BadRequest();
            }

            var materialGroup = await _materialService.UpdateMaterialGroupAsync(request);
            if (materialGroup == null)
            {
                return NotFound();
            }
            return Ok(materialGroup);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMaterialGroup(int id)
        {
            var result = await _materialService.DeleteMaterialGroupAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}