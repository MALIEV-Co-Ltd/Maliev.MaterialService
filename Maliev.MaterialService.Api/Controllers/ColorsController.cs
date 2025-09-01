using Maliev.MaterialService.Api.DTOs;
using Maliev.MaterialService.Api.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Asp.Versioning;

namespace Maliev.MaterialService.Api.Controllers
{
    [ApiController]
    [Route("v{version:apiVersion}/colors")]
    [ApiVersion("1.0")]
    public class ColorsController : ControllerBase
    {
        private readonly IMaterialServiceService _materialService;

        public ColorsController(IMaterialServiceService materialService)
        {
            _materialService = materialService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ColorDto>>> GetColors()
        {
            var colors = await _materialService.GetAllColorsAsync();
            return Ok(colors);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ColorDto>> GetColor(int id)
        {
            var color = await _materialService.GetColorByIdAsync(id);
            if (color == null)
            {
                return NotFound();
            }
            return Ok(color);
        }

        [HttpPost]
        public async Task<ActionResult<ColorDto>> CreateColor(CreateColorRequest request)
        {
            var color = await _materialService.CreateColorAsync(request);
            return CreatedAtAction(nameof(GetColor), new { id = color.Id }, color);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ColorDto>> UpdateColor(int id, UpdateColorRequest request)
        {
            if (id != request.Id)
            {
                return BadRequest();
            }

            var color = await _materialService.UpdateColorAsync(request);
            if (color == null)
            {
                return NotFound();
            }
            return Ok(color);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteColor(int id)
        {
            var result = await _materialService.DeleteColorAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}