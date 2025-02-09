using ELTEKAps.Management.Domain.Customers;
using ELTEKAps.Management.Domain.Operations;

namespace ELTEKAps.Management.ApplicationServices.Customers.Update
{
    public interface IUpdateCustomerService
    {
        Task<OperationResult> RequestUpdateCustomer(CustomerModel customerModel, OperationDetails operationDetails);
        Task UpdateCustomer(CustomerModel customerModel);
    }
}
