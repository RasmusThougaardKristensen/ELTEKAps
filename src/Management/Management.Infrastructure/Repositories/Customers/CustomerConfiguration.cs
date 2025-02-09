using ELTEKAps.Management.Infrastructure.Repositories.BaseRepository;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ELTEKAps.Management.Infrastructure.Repositories.Customers;

public class CustomerConfiguration : BaseEntityConfiguration<CustomerEntity>
{
    public override void Configure(EntityTypeBuilder<CustomerEntity> builder)
    {
        base.Configure(builder);

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id).IsRequired().ValueGeneratedNever();
        builder.Property(c => c.CustomerName).IsRequired().HasMaxLength(255);
        builder.Property(c => c.PhoneNumber).IsRequired().HasMaxLength(20);
        builder.Property(c => c.Email).IsRequired().HasMaxLength(255);
    }
}
