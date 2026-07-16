using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RotaCerta.Domain.Models;
using RotaCerta.Domain.Viagens;
using RotaCerta.Infraestructure.Context.Identity;
using System.Reflection;

namespace RotaCerta.Infraestructure.Context
{
    public class DbRotaCertaContext : IdentityDbContext<ApplicationUser>
    {
        public DbRotaCertaContext(DbContextOptions<DbRotaCertaContext> options)
          : base(options)
        {
        }

        public DbSet<Motorista> Motoristas { get; set; }
        public DbSet<Viagem> Viagens { get; set; }
        public DbSet<Entrega> Entregas { get; set; }
        public DbSet<Veiculo> Veiculos { get; set; }
        public DbSet<Manutencao> Manutencoes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Aplica mappings
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(modelBuilder);
        }
    }
}
