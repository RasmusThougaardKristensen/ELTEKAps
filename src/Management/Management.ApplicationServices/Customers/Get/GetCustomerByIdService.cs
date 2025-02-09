using ELTEKAps.Management.ApplicationServices.Repositories.Customers;
using ELTEKAps.Management.Domain.Customers;
using Microsoft.Extensions.Logging;

namespace ELTEKAps.Management.ApplicationServices.Customers.Get
{
    public class GetCustomerByIdService : IGetCustomerByIdService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ILogger<GetCustomerByIdService> _logger;

        public GetCustomerByIdService(
            ICustomerRepository customerRepository,
            ILogger<GetCustomerByIdService> logger)
        {
            _customerRepository = customerRepository;
            _logger = logger;
        }

        public async Task<CustomerModel?> GetCustomerById(Guid customerId)
        {
            try
            {
                _logger.LogInformation("Fetching customer by ID: {CustomerId}", customerId);

                var customer = await _customerRepository.GetById(customerId);

                if (customer == null)
                {
                    _logger.LogWarning("Customer with ID: {CustomerId} not found", customerId);
                }
                else
                {
                    _logger.LogInformation("Customer with ID: {CustomerId} retrieved successfully", customerId);
                }

                return customer;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching customer with ID: {CustomerId}", customerId);
                throw new GetCustomersServiceException($"Error fetching customer with ID: {customerId}", ex);
            }
        }
    }
}
