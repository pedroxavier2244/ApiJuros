using ApiJuros.Domain;

namespace ApiJuros.Application.Interfaces
{
    public interface ITokenService
    {
        Task<string> CreateTokenAsync(ApplicationUser user);
    }
}