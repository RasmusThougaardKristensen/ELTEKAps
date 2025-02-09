using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ELTEKAps.Management.Infrastructure.Repositories.BaseRepository;
public abstract class BaseEntityConfiguration<T> : IEntityTypeConfiguration<T> where T : BaseEntity
{
    private const string ClusterIdName = "ClusterId";

    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        builder.HasKey(e => e.Id).IsClustered(false);
        builder.Property(e => e.CreatedUtc).IsRequired();

        // Setting the Clustering Key
        // As Mentioned in: https://stackoverflow.com/a/11938495
        builder.Property<int>(ClusterIdName)
            .ValueGeneratedOnAdd()
            .Metadata
            .SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
        builder.HasIndex(ClusterIdName)
            .IsUnique()
            .IsClustered();
    }
}