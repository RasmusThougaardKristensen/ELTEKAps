using ELTEKAps.Management.ApplicationServices.Repositories.Customers;
using ELTEKAps.Management.Domain.Customers;
using ELTEKAps.Management.Infrastructure.Repositories.BaseRepository;
using ELTEKAps.Management.Infrastructure.Repositories.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ELTEKAps.Management.Infrastructure.Repositories.Customers
{
    public class CustomerRepository : BaseRepository<CustomerModel, CustomerEntity>, ICustomerRepository
    {
        public CustomerRepository(TaskContext context) : base(context)
        {
        }

        public async Task<CustomerModel?> GetCustomerByEmail(string email)
        {
            var customer = await GetCustomerDbSet()
                .Where(x => x.Email == email)
                .FirstOrDefaultAsync();

            return customer is null ? null : Map(customer);
        }

        public async Task<IEnumerable<CustomerModel>> GetNonDeletedCustomers()
        {
            var customers = await GetCustomerDbSet()
                .Where(customer => customer.Deleted == false)
                .ToListAsync();

            return customers.Select(Map);
        }

        private DbSet<CustomerEntity> GetCustomerDbSet()
        {
            if (Context.Customers is null)
                throw new InvalidOperationException("Database configuration not setup correctly");
            return Context.Customers;
        }

        protected override CustomerModel Map(CustomerEntity entity)
        {
            return CustomerMapper.Map(entity);
        }

        protected override CustomerEntity Map(CustomerModel model)
        {
            return CustomerMapper.Map(model);
        }
    }
}
