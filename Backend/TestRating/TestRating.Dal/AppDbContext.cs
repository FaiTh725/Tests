using Application.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TestRating.Dal.Configurations;
using TestRating.Domain.Entities;

namespace TestRating.Dal
{
    public class AppDbContext : DbContext
    {
        private readonly IConfiguration configuration;

        public AppDbContext(
            DbContextOptions options,
            IConfiguration configuration):
            base(options)
        {
            this.configuration = configuration;
        }

        public DbSet<Profile> Profiles { get; set; }

        public DbSet<Feedback> Feedbacks { get; set; }

        public DbSet<FeedbackReport> Reports { get; set; }

        public DbSet<FeedbackReview> Reviews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new FeedbackConfiguration());
            modelBuilder.ApplyConfiguration(new FeedbackReportConfiguration());
            modelBuilder.ApplyConfiguration(new FeedbackReviewConfiguration());
            modelBuilder.ApplyConfiguration(new ProfileConfiguration());
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var postgressConnection = configuration
                .GetConnectionString("PostgressConnection") ??
                throw new AppConfigurationException("Postgress connection string");

            optionsBuilder.UseNpgsql(postgressConnection);
        }
    }
}
