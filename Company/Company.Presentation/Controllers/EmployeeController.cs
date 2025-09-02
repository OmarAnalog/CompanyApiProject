using Company.Presentation.ActionFilters;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DTO;
using Shared.RequestFeatures;
using System.Text.Json;

namespace Company.Presentation.Controllers
{
    [ApiController]
    [Route("api/Company/{companyId}/Employee")]
    public class EmployeeController:ControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public EmployeeController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }
        [HttpGet]
        public async Task<IActionResult> GetEmployeesAsync(Guid companyId, [FromQuery]EmployeeRequestParameters employeeParams)
        {
            var pagedResult = await _serviceManager.EmployeeService.GetEmployees(companyId,employeeParams,false);
            Response.Headers.Add("X-Pagination",JsonSerializer.Serialize(pagedResult.Item2));
            return Ok(pagedResult.Item1);
        }
        [HttpGet("{id:guid}",Name ="GetEmployeeForCompany")]
        public async Task<IActionResult> GetEmployeeForCompanyAsync(Guid companyId, Guid id)
        {
            var employee = await _serviceManager.EmployeeService.GetEmployee(companyId, id,
            trackChanges: false);
            return Ok(employee);
        }
        [HttpPost]
        [ServiceFilter(typeof(ActionFiltersAttribute))]
        public async Task<IActionResult> CreateEmployeeAsync(Guid companyId, [FromBody] EmployeeForCreationDto employee) { 
            var createdEmployee=await _serviceManager.EmployeeService.CreateEmployee(companyId, employee,false);
            return CreatedAtRoute("GetEmployeeForCompany", new { companyId,id=createdEmployee.Id }, createdEmployee);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployeeAsync(Guid companyId, Guid id)
        {
            await _serviceManager.EmployeeService.DeleteEmployee(companyId, id,
           trackChanges: false);
            return NoContent();
        }
        [HttpPut("{id}")]
        [ServiceFilter(typeof(ActionFiltersAttribute))]
        public async Task<IActionResult> UpdateEmployeeAsync(Guid companyId,Guid id,EmployeeForUpdateDto employee)
        {
            await _serviceManager.EmployeeService.UpdateEmployee(companyId, id, employee, false, true);
            return NoContent();
        }
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateEmployeePartiallyAsync(Guid companyId,Guid id, JsonPatchDocument<EmployeeForUpdateDto> pathchDoc)
        {

            if (pathchDoc is null)
            {
                return BadRequest("pathDoc object is null");
            }
            var result = await _serviceManager.EmployeeService.GetEmployeeForPatch(companyId, id, false, true);
            pathchDoc.ApplyTo(result.employeeToPatch,ModelState);
            TryValidateModel(result.employeeToPatch);
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }
            await _serviceManager.EmployeeService.SaveChangesForPatch(result.employeeToPatch, result.employeeEntity);
            return NoContent();
        }
    }
}
