using CompanyEmployees.MVC.Security;
using Contracts;
using Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Repository;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
//https://www.mikesdotnetting.com/article/345/localisation-in-asp-net-core-razor-pages-cultures

namespace CompanyEmployees.MVC
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<RepositoryContext>(opt => opt.UseSqlServer(Configuration.GetConnectionString("sqlConnection")));
            services.AddDbContext<SecurityDbContext>(opt => opt.UseSqlServer(Configuration.GetConnectionString("securityConnection")));
            services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<SecurityDbContext>();
            services.ConfigureApplicationCookie(opt =>
            {
                opt.LoginPath = "/Security/SignIn";
                opt.AccessDeniedPath = "/Security/AccessDenied";
            });
            services.AddSession();

            services.AddScoped<IRepositoryManager, RepositoryManager>();
            services.AddControllersWithViews();
            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                 {
                    new CultureInfo("en-US"),
                    new CultureInfo("nl")
                };
                options.DefaultRequestCulture = new RequestCulture("nl");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            var localizationOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>().Value;
            app.UseRequestLocalization(localizationOptions);
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
