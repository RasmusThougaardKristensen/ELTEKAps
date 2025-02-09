using ELTEKAps.Management.Domain.Customers;
using ELTEKAps.Management.Domain.Operations;

namespace ELTEKAps.Management.ApplicationServices.Customers.Create
{
    public interface ICreateCustomerService
    {
        /// <summary>
        /// Queues an operation to create a new customer.
        /// </summary>
        /// <param name="customerModel">CustomerModel containing data to create.</param>
        /// <param name="operationDetails">Operation details such as who initiated it.</param>
        /// <returns>An OperationResult indicating the outcome.</returns>
        Task<OperationResult> RequestCreateCustomer(string customerName, string phoneNumber, string email, OperationDetails operationDetails);

        /// <summary>
        /// Actually performs creation of a customer in the repository.
        /// </summary>
        /// <param name="customerModel">CustomerModel to persist in the repository.</param>
        Task CreateCustomer(CustomerModel customerModel);
    }
}
