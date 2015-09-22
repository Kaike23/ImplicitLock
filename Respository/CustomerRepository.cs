namespace Repository
{
    using Infrastructure.Mapping;
    using Infrastructure.UnitOfWork;
    using Model.Customers;
    using Repository.Base;

    public class CustomerRepository : RepositoryBase<Customer>, ICustomerRepository
    {
        public CustomerRepository(IUnitOfWork uow, IDataMapper mapper) : base(uow, mapper) { }

        protected override string TableName { get { return "Customers"; } }
    }
}

