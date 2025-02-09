using ELTEKAps.Management.Domain.Customers;

namespace ELTEKAps.Management.TestFixtures.Customers;

public static class CustomerModelFixture
{
    public static CustomerModelBuilder Builder() => new();

    public sealed class CustomerModelBuilder
    {
        private Guid _id = Guid.NewGuid();
        private DateTime _createdUtc = DateTime.UtcNow.AddMonths(-1);
        private DateTime _modifiedUtc = DateTime.UtcNow;
        private bool _deleted = false;
        private string _customerName = "Default Customer";
        private string _phoneNumber = "+4578658798";
        private string _email = "test@example.com";

        internal CustomerModelBuilder() { }

        public CustomerModel Build()
        {
            return new CustomerModel(
                _id,
                _createdUtc,
                _modifiedUtc,
                _deleted,
                _customerName,
                _phoneNumber,
                _email
            );
        }

        public CustomerModelBuilder WithId(Guid id)
        {
            _id = id;
            return this;
        }

        public CustomerModelBuilder WithCreatedUtc(DateTime createdUtc)
        {
            _createdUtc = createdUtc;
            return this;
        }

        public CustomerModelBuilder WithModifiedUtc(DateTime modifiedUtc)
        {
            _modifiedUtc = modifiedUtc;
            return this;
        }

        public CustomerModelBuilder WithDeleted(bool deleted = true)
        {
            _deleted = deleted;
            return this;
        }

        public CustomerModelBuilder WithName(string name)
        {
            _customerName = name;
            return this;
        }

        public CustomerModelBuilder WithPhoneNumber(string phoneNumber)
        {
            _phoneNumber = phoneNumber;
            return this;
        }

        public CustomerModelBuilder WithEmail(string email)
        {
            _email = email;
            return this;
        }
    }
}
