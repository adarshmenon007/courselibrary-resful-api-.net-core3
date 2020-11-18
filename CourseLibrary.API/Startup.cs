using CourseLibrary.API.DbContexts;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
           services.AddControllers();
             
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
