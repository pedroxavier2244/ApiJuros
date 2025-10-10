using ApiJuros.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ApiJuros.Infrastructure.Persistence
{
    public class ApiJurosDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApiJurosDbContext(DbContextOptions<ApiJurosDbContext> options) : base(options)
        {
        }

        public DbSet<Simulation> Simulations { get; set; } = null!;
    }
}