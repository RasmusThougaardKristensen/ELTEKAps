using ELTEKAps.Management.Domain.Operations;

namespace ELTEKAps.Management.ApplicationServices.Customers.SoftDelete
{
    public interface ISoftDeleteCustomerService
    {
        Task<OperationResult> RequestSoftDeleteCustomer(Guid customerId, OperationDetails operationDetails);
        Task SoftDeleteCustomer(Guid customerId);
    }
}
