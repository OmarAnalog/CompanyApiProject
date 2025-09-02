using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.RequestFeatures;
using System.Text.Json;

namespace Company.Presentation.Controllers
{
    [Route("api/company")]
    [ApiController]
    [ApiVersion("2.0")]
    public class CompanyV2Controller:ControllerBase
    {
        private readonly IServiceManager serviceManager;

        public CompanyV2Controller(IServiceManager serviceManager)
        {
            this.serviceManager = serviceManager;
        }
        [HttpGet]
        public async Task<IActionResult> GetCompaniesV2Async([FromQuery] CompanyRequestParameters requestParameters)
        {
            var companies = await serviceManager.CompanyService.GetAllCompanies(requestParameters, false);
            var companiesV2 = companies.Item1.Select(x => $"{x.Name} V2");
            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(companies.Item2));
            return Ok(companiesV2);
        }
    }
}
