using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestRating.Domain.Entities;

namespace TestRating.Dal.Configurations
{
    public class ProfileConfiguration :
        IEntityTypeConfiguration<Profile>
    {
        public void Configure(EntityTypeBuilder<Profile> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.Email)
                .IsUnique();

            builder.Property(x => x.Name)
                .IsRequired();

            builder.HasMany(x => x.FeedBacks)
                .WithOne(x => x.Owner)
                .HasForeignKey(x => x.OwnerId);

            builder.HasMany(x => x.Reviews)
                .WithOne(x => x.Owner)
                .HasForeignKey(x => x.OwnerId);
        }
    }
}
