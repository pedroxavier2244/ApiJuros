using ApiJuros.Application.Interfaces;
using ApiJuros.Domain;
using ApiJuros.Infrastructure.Persistence;

namespace ApiJuros.Infrastructure.Repositories
{
    public class SimulationRepository : ISimulationRepository
    {
        private readonly ApiJurosDbContext _context;

        public SimulationRepository(ApiJurosDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Simulation simulation)
        {
            await _context.Simulations.AddAsync(simulation);
            await _context.SaveChangesAsync();
        }
    }
}