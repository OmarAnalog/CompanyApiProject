using Company.Presentation.ActionFilters;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DTO;

namespace Company.Presentation.Controllers
{
    [ApiController]
    [Route("api/token")]
    public class TokenController:ControllerBase
    {
        private readonly IServiceManager _service;

        public TokenController(IServiceManager service)
        {
            _service = service;
        }
        [HttpPost("refresh")]
        [ServiceFilter(typeof(ActionFiltersAttribute))]
        public async Task<IActionResult> Refresh([FromBody] TokenDto tokenDto)
        {
            var tokenDtoToReturn = await _service.AuthenticationService.UpdateToken(tokenDto);
            return Ok(tokenDtoToReturn);
        }
    }
}
