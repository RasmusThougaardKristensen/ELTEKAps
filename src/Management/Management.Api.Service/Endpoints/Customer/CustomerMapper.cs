using ELTEKAps.Management.Domain.Customers;

namespace ELTEKAps.Management.Api.Service.Endpoints.Customer;

public static class CustomerMapper
{
    public static CustomerResponse ToResponseModel(CustomerModel task)
    {
        return new CustomerResponse(
            task.Id,
            task.CustomerName,
            task.PhoneNumber,
            task.Email
        );
    }
}
