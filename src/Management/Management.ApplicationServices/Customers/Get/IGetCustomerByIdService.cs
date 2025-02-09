using ELTEKAps.Management.Domain.Customers;

namespace ELTEKAps.Management.ApplicationServices.Customers.Get
{
    public interface IGetCustomerByIdService
    {
        /// <summary>
        /// Fetch a Customer by its ID (returns null if not found).
        /// </summary>
        Task<CustomerModel?> GetCustomerById(Guid customerId);
    }
}
