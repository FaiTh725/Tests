using Application.Shared.Exceptions;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TestRating.Dal.Configurations;
using TestRating.Dal.Interceptors;
using TestRating.Domain.Entities;

namespace TestRating.Dal
{
    public class AppDbContext : DbContext
    {
        private readonly IConfiguration configuration;
        private readonly SoftDeleteInterceptor softDeleteInterceptor;
        private readonly DomainEventsInterceptor domainEventsInterceptor;

        public AppDbContext(
            DbContextOptions<AppDbContext> options,
            SoftDeleteInterceptor softDeleteInterceptor,
            DomainEventsInterceptor domainEventsInterceptor,
            IConfiguration configuration):
            base(options)
        {
            this.configuration = configuration;
            this.softDeleteInterceptor = softDeleteInterceptor;
            this.domainEventsInterceptor = domainEventsInterceptor;
        }

        public DbSet<Profile> Profiles { get; set; }

        public DbSet<Feedback> Feedbacks { get; set; }

        public DbSet<FeedbackReport> Reports { get; set; }

        public DbSet<FeedbackReview> Reviews { get; set; }

        public DbSet<FeedbackReply> Replies { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new FeedbackConfiguration());
            modelBuilder.ApplyConfiguration(new FeedbackReportConfiguration());
            modelBuilder.ApplyConfiguration(new FeedbackReviewConfiguration());
            modelBuilder.ApplyConfiguration(new ProfileConfiguration());

            modelBuilder.AddInboxStateEntity();
            modelBuilder.AddOutboxMessageEntity();
            modelBuilder.AddOutboxStateEntity();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var postgressConnection = configuration
                .GetConnectionString("PostgressConnection") ??
                throw new AppConfigurationException("Postgress connection string");

            optionsBuilder
                .UseNpgsql(postgressConnection)
                .AddInterceptors(softDeleteInterceptor, domainEventsInterceptor);
        }
    }
}
