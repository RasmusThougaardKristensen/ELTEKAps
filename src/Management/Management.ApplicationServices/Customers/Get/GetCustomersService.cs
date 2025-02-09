using ELTEKAps.Management.ApplicationServices.Repositories.Customers;
using ELTEKAps.Management.Domain.Customers;
using Microsoft.Extensions.Logging;

namespace ELTEKAps.Management.ApplicationServices.Customers.Get
{
    public class GetCustomersService : IGetCustomersService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ILogger<GetCustomersService> _logger;

        public GetCustomersService(
            ICustomerRepository customerRepository,
            ILogger<GetCustomersService> logger)
        {
            _customerRepository = customerRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<CustomerModel>> GetCustomers()
        {
            try
            {
                _logger.LogInformation("Fetching all customers");

                var customers = await _customerRepository.GetNonDeletedCustomers();
                _logger.LogInformation("Fetched {Count} customers", customers.Count());

                return customers;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching customers");
                throw new GetCustomersServiceException("An error occurred while fetching customers.", ex);
            }
        }
    }
}
