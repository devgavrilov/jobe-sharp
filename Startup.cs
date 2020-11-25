using System;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.MemoryStorage;
using JobeSharp.Languages;
using JobeSharp.Model;
using JobeSharp.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
                .UseMemoryStorage());

            services.AddHangfireServer(options =>
            {
                options.WorkerCount = Environment.ProcessorCount;
            });

            services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase(databaseName: "Test"));
            
            services.AddSingleton<FileCache>();
            services.AddSingleton<LanguageRegistry>();
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

            app.UseAuthorization();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHangfireDashboard(new DashboardOptions { Authorization = new IDashboardAuthorizationFilter[]{  }});
            });
        }
    }
}