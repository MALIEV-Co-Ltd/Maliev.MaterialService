using System.ComponentModel.DataAnnotations;

namespace Maliev.MaterialService.Api.DTOs
{
    public class CreateMaterialGroupRequest
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? Description { get; set; }
    }
}