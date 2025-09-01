using System.ComponentModel.DataAnnotations;

namespace Maliev.MaterialService.Api.DTOs
{
    public class UpdateSurfaceFinishRequest
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;
    }
}