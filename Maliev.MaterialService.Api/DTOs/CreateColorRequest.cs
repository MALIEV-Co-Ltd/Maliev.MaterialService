using System.ComponentModel.DataAnnotations;

namespace Maliev.MaterialService.Api.DTOs
{
    public class CreateColorRequest
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;
    }
}