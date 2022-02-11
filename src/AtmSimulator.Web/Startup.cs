using System;
using AtmSimulator.Web.Database;
using AtmSimulator.Web.Middlewares;
using AtmSimulator.Web.Models.Application;
using AtmSimulator.Web.Models.Domain;
using CSharpFunctionalExtensions;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AtmSimulator.Web
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
            services
                .AddControllersWithViews()
                .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Startup>());

            services.AddDbContext<AtmSimulatorDbContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("AtmSimulator")));

            services.AddTransient<TransferService>();
            services.AddTransient<PaymentCardGenerator>();
            services.AddTransient<IRandomGenerator, BasicRandomGenerator>();
            services.AddTransient<IDateTimeProvider, UtcDateTimeProvider>();
            services.AddTransient<IFinancialInformationService, FinancialInformationService>();
            services.AddTransient<IFinancialInstitutionService, FinancialInstitutionService>();
            services.AddTransient<IFinancialTransferSystemService, FinancialTransferSystemService>();

            services.AddTransient<IAccountRepository, SqlAccountRespository>();
            services.AddTransient<IAtmRepository, SqlAtmRepository>();
            services.AddTransient<ICustomerRepository, SqlCustomerRepository>();
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

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseMiddleware<CorrelationIdResponderMiddleware>();

            app.UseMiddleware<CurrentDateTimeProviderMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
