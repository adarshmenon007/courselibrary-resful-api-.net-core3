using AutoMapper;
using CourseLibrary.API.DbContexts;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace CourseLibrary.API
{
    public class Startup
    {
        // In the startup class the configuration object is injected that allows us to access the configuration settings,
        // like settings we might store in appsettings.json
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // This method is used to add services to the built-in dependency injection container and to configure those services.
        public void ConfigureServices(IServiceCollection services)
        {
            // Adds framework services used when building APIs to the container so they can be sued for dependency injection.
            // AddControllers() method accepts an action to configure.
            // We will configure this service to return
            services.AddControllers(setupAction => 
            {
                setupAction.ReturnHttpNotAcceptable = true; // Default value is false, which will return the default media type (e.g JSON) when requested.

                // Adding steps like these will support additional format for output.
                //setupAction.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());

            }).AddXmlDataContractSerializerFormatters() // Support XML format for output. If not added will give a 406 (Not Acceptable) error. This is the preferred way for adding formatters.
             .ConfigureApiBehaviorOptions(setupAction =>
              {
                  setupAction.InvalidModelStateResponseFactory = context =>
                  {
                      // create a problem details object
                      var problemDetailsFactory = context.HttpContext.RequestServices
                          .GetRequiredService<ProblemDetailsFactory>();
                      var problemDetails = problemDetailsFactory.CreateValidationProblemDetails(
                              context.HttpContext,
                              context.ModelState);

                      // add additional info not added by default
                      problemDetails.Detail = "See the errors field for details.";
                      problemDetails.Instance = context.HttpContext.Request.Path;

                      // find out which status code to use
                      var actionExecutingContext =
                            context as Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext;

                      // if there are modelstate errors & all keys were correctly
                      // found/parsed we're dealing with validation errors
                      if ((context.ModelState.ErrorCount > 0) &&
                          (actionExecutingContext?.ActionArguments.Count == context.ActionDescriptor.Parameters.Count))
                      {
                          problemDetails.Type = "https://courselibrary.com/modelvalidationproblem";
                          problemDetails.Status = StatusCodes.Status422UnprocessableEntity;
                          problemDetails.Title = "One or more validation errors occurred.";

                          return new UnprocessableEntityObjectResult(problemDetails)
                          {
                              ContentTypes = { "application/problem+json" }
                          };
                      }

                      // if one of the keys wasn't correctly found / couldn't be parsed
                      // we're dealing with null/unparsable input
                      problemDetails.Status = StatusCodes.Status400BadRequest;
                      problemDetails.Title = "One or more errors on input occurred.";
                      return new BadRequestObjectResult(problemDetails)
                      {
                          ContentTypes = { "application/problem+json" }
                      };
                  };
              });

            // AddAutoMapper() Method allows us to input a set of assemblies.  
            // It is these assemblies that will automatically get scanned for profiles that contain mapping configurations.
            // By calling into AppDomain.CurrentDomain.GetAssemblies() we are loading profiles from all assemblies in the current domain.
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddScoped<ICourseLibraryRepository, CourseLibraryRepository>();

            services.AddDbContext<CourseLibraryContext>(options =>
            {
                options.UseSqlServer(
                    @"Server=(localdb)\mssqllocaldb;Database=CourseLibraryDB;Trusted_Connection=True;");
            }); 
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        // This method is used to specify how an ASP.NET application respond to individual request.
        // I.e. this is where we can configure the request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(appBuilder => 
                {
                    appBuilder.Run(async context =>
                    {
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync("An unexpected fault happened. Try again later");

                    });
                });
            }

            app.UseRouting(); // Related to how a request is routed to a controller action.

            app.UseAuthorization(); // Adds authorization capability to the request pipeline. We need this for securing the API. If not configured it will allow anonymous access.

            // Related to how a request is routed to a controller action. 
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers(); // Related to how a request is routed to a controller action. 
            });
        }
    }
}
