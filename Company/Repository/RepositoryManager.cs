using Contracts;

namespace Repository
{
    public class RepositoryManager : IRepositoryManager
    {
        private readonly Context _context;
        private Lazy<ICompanyRepository> companyRepository;

        private Lazy<IEmployeeRepository> employeeRepository;
        public RepositoryManager(Context context)
        {
            _context = context;
            companyRepository = new Lazy<ICompanyRepository>(() => new CompanyRepository(_context));
            employeeRepository = new Lazy<IEmployeeRepository>(() => new EmployeeRepository(_context));
        }

        public ICompanyRepository Company => companyRepository.Value;

        public IEmployeeRepository Employee => employeeRepository.Value;

        public void Save()
        => _context.SaveChanges();

        public async Task SaveAsync()
        => await _context.SaveChangesAsync();
    }
}
