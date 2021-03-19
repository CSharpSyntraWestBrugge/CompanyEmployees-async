using Contracts;
using Entities;
using LoggerService;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using NLog;
using Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CompanyEmployees
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));

        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<RepositoryContext>(opts => opts.UseSqlServer(Configuration.GetConnectionString("sqlConnection"),
                 b => b.MigrationsAssembly("CompanyEmployees")));
           // services.AddScoped<ILoggerManager, LoggerManager>(); // nieuw object per HTTP request
            services.AddScoped<IRepositoryManager, RepositoryManager>();
            //services.AddSingleton<ILoggerManager, LoggerManager>(); 
            //1 keer bij opstarten van Web APi een object maken, die wordt gebruikt zolang de web app runt
            //services.AddTransient<ILoggerManager, LoggerManager>(); 
            //telkens als er een object vereist is (dus meerdere keer new object van LoggerManager binnen 1 HTTP request is mogelijk)
            services.AddControllers();
            services.AddSwaggerGen(s =>
            {
                s.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Companies & Employees API",
                    Version = "v1"
                });
            });
            services.AddAutoMapper(typeof(Startup));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseSwagger();
            app.UseSwaggerUI(s =>
            {
                s.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");

            });

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
