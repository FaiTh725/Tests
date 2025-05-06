using Application.Shared.Exceptions;
using Authorization.Dal.Configurations;
using Authorization.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Authorization.Dal
{
    public class AppDbContext : DbContext
    {
        private readonly IConfiguration configuration;  

        public AppDbContext(
            DbContextOptions options, 
            IConfiguration configuration) :
            base(options)
        {
            this.configuration = configuration;
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new RefreshTokenConfigurations());
            modelBuilder.ApplyConfiguration(new RoleConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var npgConnection = configuration
                .GetConnectionString("NpgConnection") ??
                throw new AppConfigurationException("Postgress ConnectionString");

            optionsBuilder.UseNpgsql(npgConnection);
        }
    }
}
