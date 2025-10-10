using ApiJuros.Domain;

namespace ApiJuros.Application.Interfaces
{
    public interface ISimulationRepository
    {
        Task AddAsync(Simulation simulation);
    }
}