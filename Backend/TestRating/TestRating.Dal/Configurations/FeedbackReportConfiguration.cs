using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestRating.Domain.Entities;
using TestRating.Domain.Validators;

namespace TestRating.Dal.Configurations
{
    public class FeedbackReportConfiguration :
        IEntityTypeConfiguration<FeedbackReport>
    {
        public void Configure(EntityTypeBuilder<FeedbackReport> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.ReportMessage)
                .HasMaxLength(ReportValidator.MAX_REPORT_MESSAGE_LENGTH)
                .IsRequired();

            builder.Property(x => x.CreatedTime)
                .IsRequired();

            builder.HasOne(x => x.ReportedFeedback)
                .WithMany()
                .HasForeignKey(x => x.ReportedFeedbackId)
                .IsRequired();

            builder.HasOne(x => x.Reviewer)
                .WithMany()
                .HasForeignKey(x => x.ReviewerId)
                .IsRequired();
        }
    }
}
