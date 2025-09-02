using Company.Presentation.ActionFilters;
using Contracts;
using LoggerService;
using Microsoft.EntityFrameworkCore;
using Repository;
using Service;
using Service.Contracts;
using Shared.DTO;
using Microsoft.AspNetCore.Mvc;
using Company.Presentation.Controllers;
using Microsoft.AspNetCore.Mvc.Versioning;
using Marvin.Cache.Headers;
using AspNetCoreRateLimit;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Entities.ConfigurationModels;
namespace Company.Extensions
{
    public static class ServiceExtensions
    {
        public static void AddCustomServices(this IServiceCollection services)
        {
            // Register custom services here
            // Example: services.AddScoped<IMyService, MyService>();
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .WithExposedHeaders("X-Pagination"));
                 });
            services.AddApiVersioning(opt =>
            {
                opt.ReportApiVersions = true;
                opt.AssumeDefaultVersionWhenUnspecified = true;
                opt.DefaultApiVersion = new ApiVersion(1, 0);
                opt.ApiVersionReader = new HeaderApiVersionReader("api-version");
                opt.Conventions.Controller<CompanyController>()
                .HasApiVersion(new ApiVersion(1, 0));
                opt.Conventions.Controller<CompanyV2Controller>()
                .HasDeprecatedApiVersion(new ApiVersion(2, 0));
            });
            services.AddMemoryCache();
            services.AddResponseCaching();
            services.AddHttpCacheHeaders(
                (exprOpt) => { exprOpt.MaxAge = 65;exprOpt.CacheLocation = CacheLocation.Private; },
                (validOpt) => { validOpt.MustRevalidate = true; });
            services.Configure<IISOptions>(options => { });
            services.AddSingleton<ILoggerManager, LoggerManager>();
            services.AddScoped<IRepositoryManager, RepositoryManager>();
            services.AddScoped<IServiceManager, ServiceManager>();
            services.AddScoped<ActionFiltersAttribute>();
            services.AddScoped<IDataShaper<EmployeeDto>, DataShaper<EmployeeDto>>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
        }
        public static void ConfigureRateLimitingOptions(this IServiceCollection services)
        {
            var rateLimitRules = new List<RateLimitRule>
                {
                    new()
                    {
                    Endpoint = "*",
                    Limit = 100,
                    Period = "5m"
                    }
                };
            services.Configure<IpRateLimitOptions>(opt =>
            {
                opt.GeneralRules= rateLimitRules;
            });
            services.AddSingleton<IRateLimitCounterStore,
            MemoryCacheRateLimitCounterStore>();
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
            services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
        }
        public static void ConfigureIdentity(this IServiceCollection services)
        {
            var builder = services.AddIdentity<User, IdentityRole>(o =>
            {
                o.Password.RequireDigit = true;
                o.Password.RequireLowercase = false;
                o.Password.RequireUppercase = false;
                o.Password.RequireNonAlphanumeric = false;
                o.Password.RequiredLength = 10;
                o.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<Context>()
            .AddDefaultTokenProviders();
        }
        public static void ConfigureJWT(this IServiceCollection services,IConfiguration configuration)
        {
            var jwtSettings = new JwtConfiguration();
            configuration.Bind(jwtSettings.Section, jwtSettings);
            var secretKey = Environment.GetEnvironmentVariable("SECRET2");
            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(opt =>
            {
                opt.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.ValidIssuer,
                    ValidAudience = jwtSettings.ValidAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
                };
            });
        }
        public static void AddJwtConfiguration(this IServiceCollection services,
        IConfiguration configuration) =>
            services.Configure<JwtConfiguration>(configuration.GetSection("JwtSettings"));
        public static void AddCustomConfigurations(this IServiceCollection services, IConfiguration configuration)
        {
            // Register custom configurations here
            // Example: services.Configure<MySettings>(configuration.GetSection("MySettings"));
            services.AddDbContext<Context>(options => { 
                options.UseSqlServer(configuration.GetConnectionString("sqlConnection"));
            });
        }

        public static void AddCustomMiddlewares(this IApplicationBuilder app)
        {
            // Register custom middlewares here
            // Example: app.UseMiddleware<MyCustomMiddleware>();
        }
    }
}
