using Authorization.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Authorization.Dal.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.HasKey(x => x.RoleName);

            builder.HasMany(x => x.Users)
                .WithOne(x => x.Role)
                .HasForeignKey(x => x.RoleId);
        }
    }
}
