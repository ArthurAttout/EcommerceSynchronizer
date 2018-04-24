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
using Hangfire.Storage.Monitoring;
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

            var posProvider = new POSInterfaceProvider(cfg);

            services.AddSingleton<IPOSInterfaceProvider>(posProvider);
            services.AddSingleton<IEcommerceDatabase>(new EcommerceDatabase(Configuration["ecommerceDatabaseConnectionString"],posProvider));
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

            var toDelete = new List<string>();

            foreach (QueueWithTopEnqueuedJobsDto queue in JobStorage.Current.GetMonitoringApi().Queues())
            {
                for (var i = 0; i < Math.Ceiling(queue.Length / 1000d); i++)
                {
                    JobStorage.Current.GetMonitoringApi().EnqueuedJobs(queue.Name, 1000 * i, 1000)
                        .ForEach(x => toDelete.Add(x.Key));
                }
            }

            foreach (var jobId in toDelete)
            {
                BackgroundJob.Delete(jobId);
            }
        }
    }
}
