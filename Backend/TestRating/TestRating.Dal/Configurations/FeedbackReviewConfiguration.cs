using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestRating.Domain.Entities;

namespace TestRating.Dal.Configurations
{
    public class FeedbackReviewConfiguration :
        IEntityTypeConfiguration<FeedbackReview>
    {
        public void Configure(EntityTypeBuilder<FeedbackReview> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.IsPositive)
                .IsRequired();

            builder.HasOne(x => x.Owner)
                .WithMany(x => x.Reviews)
                .HasForeignKey(x => x.OwnerId)
                .IsRequired();

            builder.HasOne(x => x.ReviewedFeedback)
                .WithMany(x => x.Reviews)
                .HasForeignKey(x => x.ReviewedFeedbackId)
                .IsRequired();
        }
    }
}
