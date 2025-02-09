using ELTEKAps.Management.Infrastructure.Repositories.BaseRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ELTEKAps.Management.Infrastructure.Repositories.Users;
public class UserConfiguration : BaseEntityConfiguration<UserEntity>
{
    public override void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        base.Configure(builder);

        builder.Property(user => user.FirebaseId).IsRequired();
        builder.Property(user => user.Name).IsRequired();
        builder.Property(user => user.Email).IsRequired();

        builder.HasMany(user => user.TaskEntities)
           .WithOne()
           .HasForeignKey(task => task.UserId)
           .OnDelete(DeleteBehavior.Cascade);
    }
}
