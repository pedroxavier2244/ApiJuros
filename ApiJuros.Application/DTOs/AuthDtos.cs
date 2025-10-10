using System.ComponentModel.DataAnnotations;

namespace ApiJuros.Application.DTOs
{
    public record RegisterDto(
        [Required]
        [EmailAddress]
        string Email,

        [Required]
        string Username,

        [Required]
        string Password
    );

    public record LoginDto(
        [Required]
        [EmailAddress]
        string Email,

        [Required]
        string Password
    );

    public record LoginResponseDto(string Token);
}
