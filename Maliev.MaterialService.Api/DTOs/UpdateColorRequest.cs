using System.ComponentModel.DataAnnotations;

namespace Maliev.MaterialService.Api.DTOs
{
    public class UpdateColorRequest
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;
    }
}