namespace ELTEKAps.Management.Domain.Customers;

public class CustomerModel : BaseModel
{
    public string CustomerName { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }

    public CustomerModel(Guid id, DateTime createdUtc, DateTime modifiedUtc, string customerName, string phoneNumber, string email)
        : base(id, createdUtc, modifiedUtc)
    {
        CustomerName = customerName;
        PhoneNumber = phoneNumber;
        Email = email;
    }

    public CustomerModel(Guid id, DateTime createdUtc, DateTime modifiedUtc, bool deleted, string customerName, string phoneNumber, string email)
        : base(id, createdUtc, modifiedUtc, deleted)
    {
        CustomerName = customerName;
        PhoneNumber = phoneNumber;
        Email = email;
    }

    public static CustomerModel Create(string customerName, string phoneNumber, string email)
    {
        return new CustomerModel(
            Guid.NewGuid(),
            DateTime.UtcNow,
            DateTime.UtcNow,
            customerName,
            phoneNumber,
            email
        );
    }
}
