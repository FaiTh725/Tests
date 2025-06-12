using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestRating.Domain.Entities;
using TestRating.Domain.Validators;

namespace TestRating.Dal.Configurations
{
    public class FeedbackReplyConfiguration :
        IEntityTypeConfiguration<FeedbackReply>
    {
        public void Configure(EntityTypeBuilder<FeedbackReply> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.SendTime)
                .IsRequired();

            builder.Property(x => x.UpdateTime)
                .IsRequired();

            builder.Property(x => x.IsDeleted)
                .HasDefaultValue(false);

            builder.HasOne(x => x.Owner)
                .WithMany()
                .HasForeignKey(x => x.OwnerId)
                .IsRequired();

            builder.Property(x => x.Text)
                .HasMaxLength(FeedbackReplyValidator.MAX_REPLY_MESSAGE_LENGTH)
                .IsRequired();

            builder.HasOne(x => x.Feedback)
                .WithMany(x => x.Replies)
                .HasForeignKey(x => x.FeedbackId)
                .IsRequired();
        }
    }
}
