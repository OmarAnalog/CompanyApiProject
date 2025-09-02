
using AutoMapper;
using Contracts;
using Entities.ConfigurationModels;
using Entities.Exceptions;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Service.Contracts;
using Shared.DTO;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
namespace Service
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly ILoggerManager loggerManager;
        private readonly IMapper mapper;
        private readonly UserManager<User> userManager;
        private readonly IOptions<JwtConfiguration> configuration;
        private User? _user;
        private readonly JwtConfiguration jwtConfiguration;
        public AuthenticationService(ILoggerManager loggerManager
            ,IMapper mapper
            ,UserManager<User> userManager
            ,IOptions<JwtConfiguration> configuration )
        {
            this.loggerManager = loggerManager;
            this.mapper = mapper;
            this.userManager = userManager;
            this.configuration = configuration;
            jwtConfiguration=configuration.Value;
        }

        public async Task<TokenDto> CreateToken(bool populateExp)
        {
            /*
             three things
            1- signing credentials
            2- claims
            3- token params
            4- 
             */
            var signingCredentials = GetSigningCredentials();
            var claims = await GetClaimsAsync();
            var jwtOptions = GenerateTokenOptions(signingCredentials, claims);
            var refreshToken = GenerateRefreshToken();
            _user.RefreshToken= refreshToken;
            if (populateExp)
                _user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
            await userManager.UpdateAsync(_user);
            var accessToken=new JwtSecurityTokenHandler().WriteToken(jwtOptions);
            return new TokenDto(accessToken, refreshToken);
        }

        public async Task<IdentityResult> RegisterUser(UserForRegistrationDto userForRegistration)
        {
            _user=mapper.Map<User>(userForRegistration);
            var result=await userManager.CreateAsync(_user, userForRegistration.Password);
            if (result.Succeeded)
                await userManager.AddToRolesAsync(_user, userForRegistration.Roles);
            return result;
        }

        public async Task<bool> ValidateUser(UserForAuthenticationDto userForAuthentication)
        {
            _user = await userManager.FindByNameAsync(userForAuthentication.UserName);
            if (_user is null)
            {
                loggerManager.LogWarn("User username or password is incorrect");
                return false;
            }
            var confirmPassword = await userManager.CheckPasswordAsync(_user, userForAuthentication.Password);
            if (!confirmPassword)
            {
                loggerManager.LogWarn("User username or password is incorrect");
            }
            return confirmPassword;
        }
        private SigningCredentials GetSigningCredentials()
        {
            var key = Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("SECRET2"));
            var secret = new SymmetricSecurityKey(key);
            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }
        private async Task<List<Claim>> GetClaimsAsync()
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name,_user.UserName)
            };
            var roles = await userManager.GetRolesAsync(_user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role,role));
            }
            return claims;
        }
        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials,List<Claim> claims)
        {
            var tokenOptions = new JwtSecurityToken(
                issuer: jwtConfiguration.ValidIssuer,
                audience: jwtConfiguration.ValidAudience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(jwtConfiguration.Expires)),
                signingCredentials:signingCredentials
                );
            return tokenOptions;
        }
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("SECRET2"))),
                ValidateLifetime = true,
                ValidIssuer = jwtConfiguration.ValidIssuer,
                ValidAudience = jwtConfiguration.ValidAudience
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out
        securityToken);

            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null ||
        !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }

        public async Task<TokenDto> UpdateToken(TokenDto token)
        {
            var principles = GetPrincipalFromExpiredToken(token.AccessToken);
            var user = await userManager.FindByNameAsync(principles.Identity.Name);
            if (user is null || user.RefreshToken!=token.RefreshToken 
                || user.RefreshTokenExpiryTime<=DateTime.Now)
                throw new RefreshTokenBadRequest();
            return await CreateToken(false);
        }
    }
}
