using ELTEKAps.Management.Infrastructure.Repositories.BaseRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ELTEKAps.Management.Infrastructure.Repositories.Photos;
public class PhotoConfiguration : BaseEntityConfiguration<PhotosEntity>
{
    public override void Configure(EntityTypeBuilder<PhotosEntity> builder)
    {
        base.Configure(builder);

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id).IsRequired().ValueGeneratedNever();
        builder.Property(p => p.PhotoData).IsRequired();

        builder.HasOne(photo => photo.TaskEntity) // A Photos belongs to one Task
            .WithMany(task => task.PhotoEntities) // A Task has many Photos
            .HasForeignKey(photos => photos.TaskId) // Foreign key in PhotosEntity
            .OnDelete(DeleteBehavior.Cascade); // Optional: Configure delete behavior
    }
}