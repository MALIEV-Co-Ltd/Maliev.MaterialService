using Maliev.MaterialService.Api.DTOs;
using Maliev.MaterialService.Api.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Asp.Versioning;

namespace Maliev.MaterialService.Api.Controllers
{
    [ApiController]
    [Route("v{version:apiVersion}/surfacefinishes")]
    [ApiVersion("1.0")]
    public class SurfaceFinishesController : ControllerBase
    {
        private readonly IMaterialServiceService _materialService;

        public SurfaceFinishesController(IMaterialServiceService materialService)
        {
            _materialService = materialService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SurfaceFinishDto>>> GetSurfaceFinishes()
        {
            var surfaceFinishes = await _materialService.GetAllSurfaceFinishesAsync();
            return Ok(surfaceFinishes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SurfaceFinishDto>> GetSurfaceFinish(int id)
        {
            var surfaceFinish = await _materialService.GetSurfaceFinishByIdAsync(id);
            if (surfaceFinish == null)
            {
                return NotFound();
            }
            return Ok(surfaceFinish);
        }

        [HttpPost]
        public async Task<ActionResult<SurfaceFinishDto>> CreateSurfaceFinish(CreateSurfaceFinishRequest request)
        {
            var surfaceFinish = await _materialService.CreateSurfaceFinishAsync(request);
            return CreatedAtAction(nameof(GetSurfaceFinish), new { id = surfaceFinish.Id }, surfaceFinish);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<SurfaceFinishDto>> UpdateSurfaceFinish(int id, UpdateSurfaceFinishRequest request)
        {
            if (id != request.Id)
            {
                return BadRequest();
            }

            var surfaceFinish = await _materialService.UpdateSurfaceFinishAsync(request);
            if (surfaceFinish == null)
            {
                return NotFound();
            }
            return Ok(surfaceFinish);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSurfaceFinish(int id)
        {
            var result = await _materialService.DeleteSurfaceFinishAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}