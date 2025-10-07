
namespace ApiJuros.Application.Interfaces
{
    public interface ITaxaJurosProvider
    {
        Task<decimal> GetTaxaJurosAtualAsync();
    }
}
