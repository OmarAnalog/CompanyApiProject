using Company.Presentation.ActionFilters;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DTO;

namespace Company.Presentation.Controllers
{
    [Route("api/authentication")]
    [ApiController]
    public class AuthenticationController:ControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public AuthenticationController(IServiceManager serviceManager)
        => _serviceManager = serviceManager;

        [HttpPost]
        [ServiceFilter(typeof(ActionFiltersAttribute))]
        public async Task<IActionResult> RegisterUser([FromBody] UserForRegistrationDto user)
        {
            var result=await _serviceManager.AuthenticationService.RegisterUser(user);
            if (!result.Succeeded) {
                foreach (var error in result.Errors)
                {
                    ModelState.TryAddModelError(error.Code,error.Description);
                }
                return BadRequest(ModelState);
            }
            return StatusCode(201);
        }
        [HttpPost("login")]
        [ServiceFilter(typeof(ActionFiltersAttribute))]
        public async Task<IActionResult> Authenticate([FromBody] UserForAuthenticationDto user)
        {
            if (!await _serviceManager.AuthenticationService.ValidateUser(user))
                return Unauthorized();
            var tokenDto = await _serviceManager.AuthenticationService.CreateToken(populateExp: true);
            return Ok(tokenDto);
        }
    }
}
