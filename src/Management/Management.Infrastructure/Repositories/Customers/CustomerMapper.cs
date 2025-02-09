using ELTEKAps.Management.Domain.Customers;

namespace ELTEKAps.Management.Infrastructure.Repositories.Customers;

internal static class CustomerMapper
{
    internal static CustomerEntity Map(CustomerModel model)
    {
        return new CustomerEntity(
            model.Id,
            model.CreatedUtc,
            model.ModifiedUtc,
            model.Deleted,
            model.CustomerName,
            model.PhoneNumber,
            model.Email
        );
    }

    internal static CustomerModel Map(CustomerEntity entity)
    {
        return new CustomerModel(
            entity.Id,
            entity.CreatedUtc,
            entity.ModifiedUtc,
            entity.Deleted,
            entity.CustomerName,
            entity.PhoneNumber,
            entity.Email
        );
    }
}
