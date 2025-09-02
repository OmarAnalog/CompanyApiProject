using Entities.Models;
using Shared.DTO;
using Shared.RequestFeatures;
using System.Dynamic;

namespace Service.Contracts
{
    public interface IEmployeeService
    {
        Task<(IEnumerable<ExpandoObject>,MetaData)> GetEmployees(Guid companyId,EmployeeRequestParameters employeeRequestParameters, bool trackChanges);
        Task<EmployeeDto> GetEmployee(Guid companyId, Guid id, bool trackChanges);
        Task<EmployeeDto> CreateEmployee(Guid companyId, EmployeeForCreationDto employee,bool trackChanges);
        Task DeleteEmployee(Guid companyId,Guid id,bool trackChanges);
        Task UpdateEmployee(Guid companyId, Guid id,EmployeeForUpdateDto employee, bool compTrackChanges, bool empTrackChanges);
        Task<(EmployeeForUpdateDto employeeToPatch, Employee employeeEntity)> GetEmployeeForPatch(Guid companyId, Guid id, bool compTrackChanges, bool empTrackChanges);
        Task SaveChangesForPatch(EmployeeForUpdateDto employeeEntity,Employee employee);
    }
}
