using ELTEKAps.Management.Infrastructure.Repositories.BaseRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ELTEKAps.Management.Infrastructure.Repositories.Operations;
internal class OperationEntityConfiguration : BaseEntityConfiguration<OperationEntity>
{
    public override void Configure(EntityTypeBuilder<OperationEntity> builder)
    {
        base.Configure(builder);

        builder.Property(p => p.RequestId)
            .HasMaxLength(255)
            .IsRequired();
        builder.Property(p => p.OperationName)
            .HasMaxLength(64)
            .IsRequired()
            .HasConversion<string>();
        builder.Property(p => p.TaskId).IsRequired();
        builder.Property(p => p.CreatedBy)
            .HasMaxLength(64)
            .IsRequired();
        builder.Property(p => p.Status)
            .HasMaxLength(16)
            .IsRequired()
            .HasConversion<string>();
        builder.Property(p => p.CompletedUtc).IsSparse();
        builder.Property(p => p.Data)
            .IsSparse();


        builder.HasIndex(p => new { p.RequestId }).IsUnique();
        builder.HasIndex(p => new { p.TaskId });
    }
}