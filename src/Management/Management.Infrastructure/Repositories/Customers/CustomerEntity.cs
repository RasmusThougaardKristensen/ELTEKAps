using ELTEKAps.Management.Infrastructure.Repositories.BaseRepository;

namespace ELTEKAps.Management.Infrastructure.Repositories.Customers;

public class CustomerEntity : BaseEntity
{
    public string CustomerName { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }

    public CustomerEntity(Guid id, DateTime createdUtc, DateTime modifiedUtc, bool deleted, string customerName, string phoneNumber, string email)
        : base(id, createdUtc, modifiedUtc, deleted)
    {
        CustomerName = customerName;
        PhoneNumber = phoneNumber;
        Email = email;
    }
}
