using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestRating.Domain.Entities;
using TestRating.Domain.Validators;

namespace TestRating.Dal.Configurations
{
    public class FeedbackConfiguration : 
        IEntityTypeConfiguration<Feedback>
    {
        public void Configure(EntityTypeBuilder<Feedback> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Text)
                .HasMaxLength(FeedbackValidator.MAX_FEEDBACK_MESSAGE_LENGTH)
                .IsRequired();

            builder.Property(x => x.SendTime)
                .IsRequired();

            builder.Property(x => x.UpdateTime)
                .IsRequired();

            builder.Property(x => x.IsDeleted)
                .HasDefaultValue(false);

            builder.HasOne(x => x.Owner)
                .WithMany(x => x.FeedBacks)
                .HasForeignKey(x => x.OwnerId)
                .IsRequired();

            builder.HasMany(x => x.Reviews)
                .WithOne(x => x.ReviewedFeedback)
                .HasForeignKey(x => x.ReviewedFeedbackId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.Rating)
                .IsRequired();

            builder.Property(x => x.TestId)
                .IsRequired();

            builder.HasIndex(x => x.TestId);

            builder.HasIndex(x => x.IsDeleted)
                .HasFilter("\"IsDeleted\" = false");

            builder.ToTable(x =>
                x.HasCheckConstraint(
                    "CK_Feedbacks_Rating",
                    $"\"Rating\" >= {FeedbackValidator.MIN_FEEDBACK_RATING} AND \"Rating\" <= {FeedbackValidator.MAX_FEEDBACK_RATING}"));

            // Automatically skip deleted records
            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
