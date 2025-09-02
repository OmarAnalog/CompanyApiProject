using Company.Presentation.ActionFilters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DTO;
using Shared.RequestFeatures;
using System.Text.Json;

namespace Company.Presentation.Controllers
{
    [Route("api/Company")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public CompanyController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }
        [HttpGet]
        [Authorize(Roles ="Manager")]
        public async Task<IActionResult> GetCompaniesAsync([FromQuery]CompanyRequestParameters companyParams)
        {
            var pagedResult = await _serviceManager.CompanyService.GetAllCompanies(companyParams,trackChanges: false);
            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(pagedResult.Item2));
            return Ok(pagedResult.Item1);
        }
        [HttpGet("{id}", Name = "GetCompany")]
        public async Task<IActionResult> GetCompanyAsync(Guid id)
        {
            var company = await _serviceManager.CompanyService.GetCompany(id, trackChanges: false);
            return Ok(company);
        }
        [HttpPost]
        [ServiceFilter(typeof(ActionFiltersAttribute))]
        public async Task<IActionResult> CreateCompanyAsync([FromBody] CompanyForCreationDto company) {
            var createdCompany = await _serviceManager.CompanyService.CreateCompany(company);

            return CreatedAtRoute("GetCompany", new {id=createdCompany.Id},createdCompany);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompanyAsync(Guid id)
        {
            await _serviceManager.CompanyService.DeleteCompany(id, false);
            return NoContent();
        }
        [HttpPut("{id}")]
        [ServiceFilter(typeof(ActionFiltersAttribute))]
        public async Task<IActionResult> UpdateCompanyAsync(Guid id, CompanyForUpdateDto company) { 
            await _serviceManager.CompanyService.UpdateCompany(id, company, true);
            return NoContent();
        }
    }
}
