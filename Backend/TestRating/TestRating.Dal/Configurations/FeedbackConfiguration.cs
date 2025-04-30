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

            builder.HasOne(x => x.Owner)
                .WithMany(x => x.FeedBacks)
                .HasForeignKey(x => x.OwnerId)
                .IsRequired();

            builder.Property(x => x.Rating)
                .IsRequired();

            builder.ToTable(x =>
                x.HasCheckConstraint(
                    "CK_Feedbacks_Rating",
                    $"\"Rating\" >= {FeedbackValidator.MIN_FEEDBACK_RATING} AND \"Rating\" <= {FeedbackValidator.MAX_FEEDBACK_RATING}"));
        }
    }
}
