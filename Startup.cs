using System;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.PostgreSql;
using JobeSharp.Languages;
using JobeSharp.Metrics;
using JobeSharp.Model;
using JobeSharp.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Prometheus;
using Prometheus.SystemMetrics;

namespace JobeSharp
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
            services.AddControllers();

            services.AddHangfire(configuration => configuration
                .UseRecommendedSerializerSettings()
                .UsePostgreSqlStorage(Environment.GetEnvironmentVariable("ConnectionString")));

            services.AddHostedService<DotNetRuntimeMetricsCollector>();

            services.AddSystemMetrics();

            services.AddMemoryCache(options =>
            {
                options.SizeLimit = 1024;
            });
            
            services.AddHangfireServer(options =>
            {
                options.WorkerCount = Environment.ProcessorCount;
            });

            services.AddDbContext<ApplicationDbContext>(options =>
                options
                    .UseNpgsql(Environment.GetEnvironmentVariable("ConnectionString") ??
                                  throw new ArgumentNullException("ConnectionString")));
            
            services.AddSingleton<FileCache>();
            services.AddSingleton<LanguageRegistry>();

            services.AddHealthChecks()
                .ForwardToPrometheus();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseHttpMetrics();

            app.UseAuthorization();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHangfireDashboard(new DashboardOptions { Authorization = new IDashboardAuthorizationFilter[]{  }});
                endpoints.MapMetrics();
            });
        }
    }
}