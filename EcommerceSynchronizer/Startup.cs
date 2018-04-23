using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EcommerceSynchronizer.Model;
using EcommerceSynchronizer.Model.Interfaces;
using EcommerceSynchronizer.Synchronizers;
using EcommerceSynchronizer.Utilities;
using Hangfire;
using Hangfire.Storage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols;

namespace EcommerceSynchronizer
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
            services.AddMvc();

            var cfg = new POSConfigurationList();   
            Configuration.Bind("PosAccess",cfg);

            services.AddSingleton<ApplicationState>();
            services.AddSingleton<Synchronizer>();
            services.AddAutoMapper();
            services.AddSingleton<IPOSInterfaceProvider>(new POSInterfaceProvider(cfg));
            services.AddSingleton<IEcommerceDatabase>(new EcommerceDatabase(Configuration["ecommerceDatabaseConnectionString"]));
            services.AddHangfire(x => x.UseSqlServerStorage(Configuration["hangfireDatabaseConnectionString"]));

        }

        private void createMapping(IMapperConfigurationExpression cfg)
        {
            throw new NotImplementedException();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
            
            app.UseHangfireDashboard();
            app.UseHangfireServer();
            using (var connection = JobStorage.Current.GetConnection())
            {
                foreach (var recurringJob in connection.GetRecurringJobs())
                {
                    RecurringJob.RemoveIfExists(recurringJob.Id);
                }
            }
        }
    }


}
