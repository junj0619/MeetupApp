using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MeetupApp.API.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MeetupApp.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {   /* It allows us to access appsettings.json configuration */
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            /* ConfigureServices method is our DI container */

            services.AddDbContext<DataContext>(x => x.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));
            services.AddControllers();
            /* This makes cors avaiable here so that we can use it middleware */
            services.AddCors();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            /*
                Confgure method to configure our HTTP request pipeline. Everything we added here are middleware.
                Middleware is software that can use to interact with our request as it's going through its journey through the pipeline.
            */
            if (env.IsDevelopment())
            {
                //return developer friendly exception page
                app.UseDeveloperExceptionPage();
            }

            // app.UseHttpsRedirection();

            app.UseRouting();

            /* In 3.0 this needs to go after the 'UseRouting' and before UseAuth & UseEndpoints */
            app.UseCors(x => x.AllowAnyOrigin()
                              .AllowAnyMethod()
                              .AllowAnyHeader());

            app.UseAuthorization();

            /* map our controller endpoint to the application so that our api knows how to read the request */
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
