using ELTEKAps.Management.Infrastructure.Repositories.BaseRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ELTEKAps.Management.Infrastructure.Repositories.Comments;
public class CommentConfiguration : BaseEntityConfiguration<CommentEntity>
{
    public override void Configure(EntityTypeBuilder<CommentEntity> builder)
    {
        base.Configure(builder);

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id).IsRequired().ValueGeneratedNever();
        builder.Property(p => p.Comment).IsRequired();

        builder.HasOne(comment => comment.TaskEntity) // A Comment belongs to one Task
            .WithMany(task => task.CommentEntities) // A Task has many Comments
            .HasForeignKey(comment => comment.TaskId) // Foreign key in CommentEntity
            .OnDelete(DeleteBehavior.Cascade); // Optional: Configure delete behavior
    }
}
