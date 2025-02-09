namespace ELTEKAps.Management.Api.Service.Endpoints.Customer;

public class CustomerResponse
{
    public Guid Id { get; }
    public string CustomerName { get; }
    public string PhoneNumber { get; }
    public string Email { get; }

    public CustomerResponse(Guid id, string customerName, string phoneNumber, string email)
    {
        Id = id;
        CustomerName = customerName;
        PhoneNumber = phoneNumber;
        Email = email;
    }
}