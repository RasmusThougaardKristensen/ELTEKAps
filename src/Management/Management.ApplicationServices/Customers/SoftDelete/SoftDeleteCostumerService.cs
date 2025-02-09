using ELTEKAps.Management.ApplicationServices.Operations;
using ELTEKAps.Management.ApplicationServices.Repositories.Customers;
using ELTEKAps.Management.Domain.Operations;
using Microsoft.Extensions.Logging;

namespace ELTEKAps.Management.ApplicationServices.Customers.SoftDelete
{
    public class SoftDeleteCustomerService : ISoftDeleteCustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IOperationService _operationService;
        private readonly ILogger<SoftDeleteCustomerService> _logger;

        public SoftDeleteCustomerService(
            ICustomerRepository customerRepository,
            IOperationService operationService,
            ILogger<SoftDeleteCustomerService> logger)
        {
            _customerRepository = customerRepository;
            _operationService = operationService;
            _logger = logger;
        }

        public async Task<OperationResult> RequestSoftDeleteCustomer(Guid customerId, OperationDetails operationDetails)
        {
            try
            {
                _logger.LogInformation("Request soft-delete for Customer with ID: {CustomerId}", customerId);

                var existingCustomer = await _customerRepository.GetById(customerId);
                if (existingCustomer == null)
                {
                    _logger.LogWarning("Customer with ID: {CustomerId} does not exist", customerId);
                    return OperationResult.InvalidState("Customer does not exist.");
                }

                if (existingCustomer.Deleted)
                {
                    return OperationResult.InvalidState("Customer is already soft-deleted.");
                }

                var operation = await _operationService.QueueOperation(
                    OperationBuilder.SoftDeleteCustomer(customerId, operationDetails.CreatedBy)
                );

                _logger.LogInformation("Operation queued with Request ID: {RequestId} for Customer ID: {CustomerId}",
                    operation.RequestId, customerId);

                await SoftDeleteCustomer(customerId);

                return OperationResult.Accepted(operation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during soft-delete request for Customer ID: {CustomerId}", customerId);
                throw new CustomerOperationException("Failed to queue and soft-delete the customer.", ex);
            }
        }

        public async Task SoftDeleteCustomer(Guid customerId)
        {
            try
            {
                _logger.LogInformation("Soft-deleting Customer with ID: {CustomerId}", customerId);

                var customer = await _customerRepository.GetById(customerId);
                if (customer == null)
                {
                    _logger.LogWarning("Customer with ID: {CustomerId} was not found for soft-delete", customerId);
                    return;
                }

                customer.SoftDelete();
                await _customerRepository.Upsert(customer);

                _logger.LogInformation("Customer with ID: {CustomerId} has been soft-deleted", customerId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error soft-deleting Customer with ID: {CustomerId}", customerId);
                throw new CustomerSoftDeleteException("Failed to soft-delete the customer.", ex);
            }
        }
    }
}
