using ELTEKAps.Management.Infrastructure.Repositories.BaseRepository;
using ELTEKAps.Management.Infrastructure.Repositories.Customers;
using ELTEKAps.Management.Infrastructure.Repositories.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ELTEKAps.Management.Infrastructure.Repositories.Tasks;
public class TaskConfiguration : BaseEntityConfiguration<TaskEntity>
{
    public override void Configure(EntityTypeBuilder<TaskEntity> builder)
    {
        base.Configure(builder);

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id).IsRequired().ValueGeneratedNever();
        builder.Property(p => p.Description).IsRequired();
        builder.Property(p => p.Status).IsRequired().HasConversion<string>();
        builder.Property(p => p.Location).IsRequired();
        builder.Property(p => p.CustomerId).IsRequired();
        builder.Property(p => p.UserId).IsRequired();
        builder.Property(p => p.Title).IsRequired();

        builder.HasMany(task => task.CommentEntities)
            .WithOne(comment => comment.TaskEntity)
            .HasForeignKey(comment => comment.TaskId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(task => task.PhotoEntities)
            .WithOne(photo => photo.TaskEntity)
            .HasForeignKey(photo => photo.TaskId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<CustomerEntity>()
            .WithMany()
            .HasForeignKey(task => task.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<UserEntity>()
            .WithMany()
            .HasForeignKey(task => task.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}