using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MeetupApp.API.Data;
using MeetupApp.API.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace MeetupApp.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {   /* It allows us to access appsettings.json configuration */
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public void ConfigureDevelopmentServices(IServiceCollection services)
        {   /* Use Sqlite in Development  */
            services.AddDbContext<DataContext>(x =>
            {
                x.UseLazyLoadingProxies();
                x.UseSqlite(Configuration.GetConnectionString("DefaultConnection"));
            }
           );

            ConfigureServices(services);
        }

        public void ConfigureProductionServices(IServiceCollection services)
        {   /* Use MySQL in production  */
            services.AddDbContext<DataContext>(x =>
            {
                x.UseLazyLoadingProxies();
                x.UseMySql(Configuration.GetConnectionString("DefaultConnection"));
            });

            ConfigureServices(services);
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            /* ConfigureServices method is our DI container */

            // services.AddDbContext<DataContext>(x => x.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));

            /* In Core 3.0 by default service use System.text.Json but we want use NewtonsoftJson features in this project */
            services.AddControllers().AddNewtonsoftJson(option =>
            {
                /* Ignore self-referencing loop detection { User => Photo => User } */
                option.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });

            /* This makes cors available here so that we can use it middleware */
            services.AddCors();

            /* Binding CloudinarySettings from appsettings.json */
            services.Configure<CloudinarySettings>(Configuration.GetSection("CloudinarySettings"));
            services.AddAutoMapper(typeof(MeetupRepository).Assembly);

            /* 
               .AddSingleton : Create single instance of our repository throughout the application 
               .AddTransient : Create one instance on each time they are requested
               .AddScoped    : Create one instance per request within the scope. 
                               Create one instance for each HTTP request but it uses the same instance of call within the same web request.
            */
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IMeetupRepository, MeetupRepository>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            /* plug in LogUserActivity pipeline that tracking user activity after request exected */
            services.AddScoped<LogUserActivity>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            /*
                Confgure method to configure our HTTP request pipeline. Everything we added here are middleware.
                Middleware is software that can use to interact with our request 
                as it's going through its journey through the pipeline.
            */
            if (env.IsDevelopment())
            {
                //return developer friendly exception page
                app.UseDeveloperExceptionPage();
            }
            else
            {
                /* Global Exception Hanldler */
                app.UseExceptionHandler(builder =>
                {
                    builder.Run(async context =>
                    {   /* Change HttpStatus Code to 500 Internal Server Error and out error into response */
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                        var error = context.Features.Get<IExceptionHandlerFeature>();
                        if (error != null)
                        {
                            context.Response.AddApplicationError(error.Error.Message);
                            await context.Response.WriteAsync(error.Error.Message);
                        }

                    });
                });
            }

            // app.UseHttpsRedirection();

            app.UseRouting();

            /* In 3.0 this needs to go after the 'UseRouting' and before UseAuth & UseEndpoints */
            app.UseCors(x => x.AllowAnyOrigin()
                              .AllowAnyMethod()
                              .AllowAnyHeader());

            app.UseAuthentication();
            app.UseAuthorization();

            /** For deployment purposes to server our angular application inside of API project **/
            /* Ask Kestrel server to serve default files (index.html, default.html inside of wwwroot folder) */
            app.UseDefaultFiles();
            /* Add abilities to our webserver to user static files. */
            app.UseStaticFiles();

            /* map our controller endpoint to the application so that our api knows how to read the request */
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                /* 
                   Anything not response by API controller then use fallback controller to hanlde 
                   In here we are using fallback controller to serve route in angular route request
                */
                endpoints.MapFallbackToController("Index", "Fallback");
            });
        }
    }
}
