using ELTEKAps.Management.ApplicationServices.Operations;
using ELTEKAps.Management.ApplicationServices.Repositories.Customers;
using ELTEKAps.Management.Domain.Customers;
using ELTEKAps.Management.Domain.Operations;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace ELTEKAps.Management.ApplicationServices.Customers.Update
{
    public class UpdateCustomerService : IUpdateCustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IOperationService _operationService;
        private readonly ILogger<UpdateCustomerService> _logger;

        public UpdateCustomerService(
            ICustomerRepository customerRepository,
            IOperationService operationService,
            ILogger<UpdateCustomerService> logger)
        {
            _customerRepository = customerRepository;
            _operationService = operationService;
            _logger = logger;
        }

        public async Task<OperationResult> RequestUpdateCustomer(CustomerModel customerModel, OperationDetails operationDetails)
        {
            try
            {
                _logger.LogInformation("Request to update Customer with ID: {CustomerId}", customerModel.Id);

                // Performs validation
                var validationErrors = ValidateCustomer(customerModel).ToList();
                if (validationErrors.Any())
                {
                    var errorMessage = string.Join("; ", validationErrors);
                    _logger.LogWarning("Validation failed for Customer with ID: {CustomerId}. Errors: {Errors}", customerModel.Id, errorMessage);
                    return OperationResult.InvalidState(errorMessage);
                }

                // Check if the customer exists
                var existingCustomer = await _customerRepository.GetById(customerModel.Id);
                if (existingCustomer == null)
                {
                    _logger.LogWarning("Customer with ID: {CustomerId} does not exist", customerModel.Id);
                    return OperationResult.InvalidState("Customer does not exist.");
                }

                // Queue the update operation
                var operation = await _operationService.QueueOperation(
                    OperationBuilder.UpdateCustomer(customerModel, operationDetails.CreatedBy)
                );

                _logger.LogInformation("Operation queued with Request ID: {RequestId} for Customer ID: {CustomerId}",
                    operation.RequestId, customerModel.Id);

                await UpdateCustomer(customerModel);

                return OperationResult.Accepted(operation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during update request for Customer ID: {CustomerId}", customerModel.Id);
                throw new CustomerOperationException("Failed to queue and update customer.", ex);
            }
        }

        public async Task UpdateCustomer(CustomerModel customerModel)
        {
            try
            {
                _logger.LogInformation("Updating Customer with ID: {CustomerId}", customerModel.Id);
                await _customerRepository.Upsert(customerModel);
                _logger.LogInformation("Customer with ID: {CustomerId} has been updated", customerModel.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating Customer with ID: {CustomerId}", customerModel.Id);
                throw new CustomerUpdateException("Failed to update customer.", ex);
            }
        }

        private static IEnumerable<string> ValidateCustomer(CustomerModel customerModel)
        {
            var errors = new List<string>();

            // Validate CustomerName
            if (string.IsNullOrWhiteSpace(customerModel.CustomerName))
            {
                errors.Add("Customer name must be provided.");
            }

            // Validate PhoneNumber (using basic international phone number regex)
            if (string.IsNullOrWhiteSpace(customerModel.PhoneNumber))
            {
                errors.Add("Phone number must be provided.");
            }
            else
            {
                var phoneRegex = new Regex(@"^\+?[1-9]\d{1,14}$");
                if (!phoneRegex.IsMatch(customerModel.PhoneNumber))
                {
                    errors.Add("Phone number is not in a valid international format.");
                }
            }

            // Validate Email
            if (string.IsNullOrWhiteSpace(customerModel.Email))
            {
                errors.Add("Email must be provided.");
            }
            else
            {
                var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                if (!emailRegex.IsMatch(customerModel.Email))
                {
                    errors.Add("Email is not in a valid format.");
                }
            }

            return errors;
        }
    }
}
