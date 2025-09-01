using System.ComponentModel.DataAnnotations;

namespace Maliev.MaterialService.Api.DTOs
{
    public class UpdateMaterialGroupRequest
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? Description { get; set; }
    }
}