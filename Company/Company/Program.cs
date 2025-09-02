
using AspNetCoreRateLimit;
using Company.Extensions;
using Contracts;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Options;
using NLog;
using NLog.LayoutRenderers;
namespace Company
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });
            NewtonsoftJsonInputFormatter GetJsonPatchInputFormatter() =>
                new ServiceCollection().AddLogging().AddMvc().AddNewtonsoftJson()
                .Services.BuildServiceProvider()
                .GetRequiredService<IOptions<MvcOptions>>().Value.InputFormatters
                .OfType<NewtonsoftJsonInputFormatter>().First();
            builder.Services.AddControllers(config=>config.InputFormatters.Insert(0,GetJsonPatchInputFormatter())).
                AddApplicationPart(typeof(Company.Presentation.AssemblyReference).Assembly);
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            var path= "C:\\Users\\EG\\source\\repos\\Company\\Company\\nlog.config";
            LogManager.Setup().LoadConfigurationFromFile(path);
            builder.Services.AddCustomServices();
            builder.Services.AddCustomConfigurations(builder.Configuration);
            builder.Services.ConfigureRateLimitingOptions();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddAuthentication();
            builder.Services.ConfigureIdentity();
            builder.Services.ConfigureJWT(builder.Configuration);
            builder.Services.AddJwtConfiguration(builder.Configuration);
            builder.Services.AddAutoMapper(typeof(Program));
            var app = builder.Build();
            var logger=app.Services.GetRequiredService<ILoggerManager>();
            app.ConfigureExceptionHandler(logger);

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else 
                app.UseHsts();

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.All
            });
            app.UseIpRateLimiting();
            app.UseCors("CorsPolicy");
            app.UseAuthentication();
            app.UseAuthorization();
            #region types of middleware
            //app.Use(async (context,next) =>
            //{
            //    Console.WriteLine("This is I'm going down\n");
            //    await next.Invoke();
            //    Console.WriteLine("This is I'm going up\n");

            //});
            //app.Run(async context =>
            //{
            //    Console.WriteLine("This is the final middleware\n");
            //    await context.Response.WriteAsync("This is the final middleware " +
            //        "Hello World from Company API!\n");
            //});
            //app.Use(async (context, next) =>
            //{
            //    await context.Response.WriteAsync("Hello from the middleware component.");
            //    await next.Invoke();
            //    Console.WriteLine($"Logic after executing the next delegate in the Use method");
            //});
            //// don't make call next.invoke after we send the response to client
            //app.Run(async context =>
            //{
            //    Console.WriteLine($"Writing the response to the client in the Run method");
            //    //context.Response.StatusCode = 200;
            //    await context.Response.WriteAsync("Hello from the middleware component.");
            //});
            //app.Use(async (context, next) =>
            //{
            //    Console.WriteLine($"Logic before executing the next delegate in the Use method");
            //    await next.Invoke();
            //    Console.WriteLine($"Logic after executing the next delegate in the Use method");
            //});
            //app.Map("/usingmapbranch", builder =>
            //{
            //    builder.Use(async (context, next) =>
            //    {
            //        Console.WriteLine("Map branch logic in the Use method before the next delegate");
            //        await next.Invoke();
            //        Console.WriteLine("Map branch logic in the Use method after the next delegate");
            //    });
            //    builder.Run(async context =>
            //    {
            //        Console.WriteLine($"Map branch response to the client in the Run method");
            //        await context.Response.WriteAsync("Hello from the map branch.");
            //    });
            //});
            //app.Run(async context =>
            //{
            //    Console.WriteLine($"ezzzzzzzay");
            //    await context.Response.WriteAsync("Hello from the middleware component.");
            //});
            //app.MapWhen(context => context.Request.Query.ContainsKey("testquerystring"), 
            //    builder =>
            //{
            //    builder.Run(async context =>
            //    {
            //        await context.Response.WriteAsync("Hello from the MapWhen branch.");

            //    });
            //});
            //app.Run(async context =>{});
            #endregion
            app.UseResponseCaching();
            app.UseHttpCacheHeaders();
            app.MapControllers();

            app.Run();
        }
    }
}
