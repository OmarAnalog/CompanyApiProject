using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Service.Contracts;
using Shared.DTO;
using Shared.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    internal class EmployeeService:IEmployeeService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        private readonly IDataShaper<EmployeeDto> _dataShaper;
        public EmployeeService(IRepositoryManager repositoryManager, ILoggerManager loggerManager, IMapper mapper, IDataShaper<EmployeeDto> dataShaper)
        {
            _logger = loggerManager;
            _mapper = mapper;
            _repositoryManager = repositoryManager;
            _dataShaper = dataShaper;
        }

        public async Task<(IEnumerable<ExpandoObject>, MetaData)> GetEmployees(Guid companyId,EmployeeRequestParameters employeeParameters, bool trackChanges)
        {
            if (employeeParameters.ValidateAge == false)
            {
                throw new MaxAgeBadRequestException();
            }
            await GetCompanyAndCheckExistence(companyId, trackChanges);
            var employees = await _repositoryManager.Employee.GetEmployees(companyId,employeeParameters,trackChanges);
            _logger.LogInfo($"Returned all employees for company with id: {companyId}.");
            var employeesDto = _mapper.Map<IEnumerable<EmployeeDto>>(employees);
            var shapedData = _dataShaper.ShapedData(employeesDto, employeeParameters.Fields);
            return (shapedData, employees.MetaData);
        }
        public async Task<EmployeeDto> GetEmployee(Guid companyId, Guid id, bool trackChanges)
        {
            await GetCompanyAndCheckExistence(companyId, trackChanges);
            var employeeDb = await GetEmployeeForCompanyAndCheckExistence(companyId, id, trackChanges);
            var employee = _mapper.Map<EmployeeDto>(employeeDb);
            return employee;
        }

        public async Task<EmployeeDto> CreateEmployee(Guid companyId, EmployeeForCreationDto employee, bool trackChanges)
        {
            await GetCompanyAndCheckExistence(companyId, trackChanges);
            var createdEmployee=_mapper.Map<Employee>(employee);
            _repositoryManager.Employee.CreateEmployee(companyId, createdEmployee);
            await _repositoryManager.SaveAsync();
            var employeeDto=_mapper.Map<EmployeeDto>(createdEmployee);
            _logger.LogInfo($"Created employee with id: {createdEmployee.Id}.");
            return employeeDto;
        }
        public async Task DeleteEmployee(Guid companyId,Guid id, bool trackChanges)
        {
            await GetCompanyAndCheckExistence(companyId, trackChanges);
            var employeeDb = await GetEmployeeForCompanyAndCheckExistence(companyId, id, trackChanges);
            _repositoryManager.Employee.DeleteEmployee(employeeDb);
            await _repositoryManager.SaveAsync();
            _logger.LogInfo($"Deleted employee with id: {employeeDb.Id}.");
        }

        public async Task UpdateEmployee(Guid companyId, Guid id,EmployeeForUpdateDto employee, bool compTrackChanges, bool empTrackChanges)
        {
            await GetCompanyAndCheckExistence(companyId, compTrackChanges);
            var employeeDb = await GetEmployeeForCompanyAndCheckExistence(companyId, id, empTrackChanges);
            _mapper.Map(employee, employeeDb);
            await _repositoryManager.SaveAsync();
            _logger.LogInfo($"Employee with id {id} was updated successfully.");
        }

        public async Task<(EmployeeForUpdateDto employeeToPatch, Employee employeeEntity)> GetEmployeeForPatch(Guid companyId, Guid id, bool compTrackChanges, bool empTrackChanges)
        {
            await GetCompanyAndCheckExistence(companyId, compTrackChanges);
            var employeeDb = await GetEmployeeForCompanyAndCheckExistence(companyId, id, empTrackChanges);
            var employeeToPatch = _mapper.Map<EmployeeForUpdateDto>(employeeDb);
            return (employeeToPatch, employeeDb);
        }

        public async Task SaveChangesForPatch(EmployeeForUpdateDto employeeEntity, Employee employee)
        {
            _mapper.Map(employeeEntity,employee);
            await _repositoryManager.SaveAsync();
            _logger.LogInfo($"Employee with id {employee.Id} was updated successfully.");
        }
        private async Task GetCompanyAndCheckExistence(Guid companyId, bool trackChanges)
        {
            var company = await _repositoryManager.Company.GetCompanyById(companyId, trackChanges);
            if (company is null)
            {
                _logger.LogInfo($"Invalid Id for a company");
                throw new CompanyNotFoundException(companyId);
            }
        }
        private async Task<Employee> GetEmployeeForCompanyAndCheckExistence(Guid companyId,Guid id, bool trackChanges)
        {
            var employee = await _repositoryManager.Employee.GetEmployee(companyId, id, trackChanges);
            if (employee is null)
            {
                _logger.LogInfo($"Invalid Id for a Employee");
                throw new EmployeeNotFoundException(id);
            }
            return employee;
        }
    }
}
