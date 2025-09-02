using System.ComponentModel.DataAnnotations;

namespace Shared.DTO
{
    public record UserForAuthenticationDto
    {
        [Required(ErrorMessage ="UserName is Required")]
        public string? UserName { get; init; }
        [Required(ErrorMessage ="Password is requried")]
        public string? Password { get; init; }
    }
}
