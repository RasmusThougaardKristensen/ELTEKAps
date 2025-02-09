using ELTEKAps.Management.Domain.Customers;

namespace ELTEKAps.Management.ApplicationServices.Repositories.Customers
{
    public interface ICustomerRepository : IBaseRepository<CustomerModel>
    {
        public Task<CustomerModel?> GetCustomerByEmail(string email);
        public Task<IEnumerable<CustomerModel>> GetNonDeletedCustomers();
    }
}
