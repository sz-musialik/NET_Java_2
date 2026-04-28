using Microsoft.EntityFrameworkCore;

namespace AplikacjaBazodanowa
{
    internal class JwstDbContext : DbContext
    {
        public DbSet<DbProgram> Programs { get; set; } // Tabela programow
        public DbSet<DbObservation> Observations { get; set; } // Tabela obserwacji

        public JwstDbContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite("Data Source=JwstDatabase.db");
        }
    }
}