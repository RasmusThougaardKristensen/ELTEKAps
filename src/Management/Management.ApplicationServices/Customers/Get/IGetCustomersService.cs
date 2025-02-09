using ELTEKAps.Management.Domain.Customers;

namespace ELTEKAps.Management.ApplicationServices.Customers.Get
{
    public interface IGetCustomersService
    {
        /// <summary>
        /// Retrieves all customers (potentially filter out deleted ones if needed).
        /// </summary>
        Task<IEnumerable<CustomerModel>> GetCustomers();
    }
}
