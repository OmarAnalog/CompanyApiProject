using Entities.Models;
using Shared.RequestFeatures;
using System.Collections;

namespace Contracts
{
    public interface IEmployeeRepository
    {
        Task<PagedList<Employee>> GetEmployees(Guid companyId, EmployeeRequestParameters employeeParameters, bool trackChanges);
        Task<Employee> GetEmployee(Guid companyId, Guid id, bool trackChanges);
        void CreateEmployee(Guid companyId,Employee employee);
        void DeleteEmployee(Employee employee);
    }
}
