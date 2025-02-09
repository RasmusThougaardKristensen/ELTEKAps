using ELTEKAps.Management.ApplicationServices.Operations;
using ELTEKAps.Management.ApplicationServices.Repositories.Customers;
using ELTEKAps.Management.Domain.Customers;
using ELTEKAps.Management.Domain.Operations;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace ELTEKAps.Management.ApplicationServices.Customers.Create
{
    public class CreateCustomerService : ICreateCustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IOperationService _operationService;
        private readonly ILogger<CreateCustomerService> _logger;

        public CreateCustomerService(
            ICustomerRepository customerRepository,
            IOperationService operationService,
            ILogger<CreateCustomerService> logger)
        {
            _customerRepository = customerRepository;
            _operationService = operationService;
            _logger = logger;
        }

        public async Task<OperationResult> RequestCreateCustomer(string customerName, string phoneNumber, string email, OperationDetails operationDetails)
        {
            try
            {
                _logger.LogInformation("Request to create customer with email: {Email}", email);

                // Check if a customer already exists for the provided email.
                var existingCustomer = await _customerRepository.GetCustomerByEmail(email);
                if (existingCustomer != null)
                {
                    _logger.LogWarning("Customer with Email: {Email} already exists", email);
                    return OperationResult.InvalidState($"Customer with email {email} already exists.");
                }

                // Validate input data using enhanced validation
                var validationErrors = ValidateCustomer(customerName, phoneNumber, email).ToList();
                if (validationErrors.Any())
                {
                    var errorMessage = string.Join("; ", validationErrors);
                    _logger.LogWarning("Invalid customer data: {ValidationMessage}", errorMessage);
                    return OperationResult.InvalidState(errorMessage);
                }

                var customerModel = CustomerModel.Create(customerName, phoneNumber, email);

                // Queue the operation
                var operation = await _operationService.QueueOperation(
                    OperationBuilder.CreateCustomer(customerModel, operationDetails.CreatedBy)
                );

                _logger.LogInformation("Operation queued with Request ID: {RequestId} for Customer ID: {CustomerId}",
                    operation.RequestId, customerModel.Id);

                await CreateCustomer(customerModel);

                // Mark the operation as completed
                await _operationService.UpdateOperationStatus(operation.RequestId, OperationStatus.Completed);
                return OperationResult.Accepted(operation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during customer creation for email: {Email}", email);
                throw new CustomerOperationException("Failed to queue and create customer.", ex);
            }
        }

        public async Task CreateCustomer(CustomerModel customerModel)
        {
            try
            {
                _logger.LogInformation("Creating customer with ID: {CustomerId}", customerModel.Id);
                await _customerRepository.Upsert(customerModel);
                _logger.LogInformation("Customer created with ID: {CustomerId}", customerModel.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating customer with ID: {CustomerId}", customerModel.Id);
                throw new CustomerCreationException("Failed to create customer.", ex);
            }
        }

        private IEnumerable<string> ValidateCustomer(string customerName, string phoneNumber, string email)
        {
            if (string.IsNullOrWhiteSpace(customerName))
                yield return "Customer name cannot be empty.";

            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                yield return "Phone number must be provided.";
            }
            else
            {
                var phoneRegex = new Regex(@"^\+?[1-9]\d{1,14}$");
                if (!phoneRegex.IsMatch(phoneNumber))
                    yield return "Phone number is not in a valid international format.";
            }

            if (string.IsNullOrWhiteSpace(email))
                yield return "Email must be provided.";
            else
            {
                var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                if (!emailRegex.IsMatch(email))
                    yield return "Email is not in a valid format.";
            }
        }

    }
}
