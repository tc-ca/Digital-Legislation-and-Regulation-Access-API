using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using LegsandRegsCS.Models;
using LegsandRegsCS.Data;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Primitives;

namespace LegsandRegsCS
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
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TC Legislation and Regulation API", Version = "v1" });
            });

            services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("AppDbContext")));

            services.AddApplicationInsightsTelemetry(Configuration);

            Program.telemetry = new TelemetryClient(
                new TelemetryConfiguration(Configuration.GetValue<string>("ApplicationInsights:InstrumentationKey")));

            Program.secretTokenHeader = Configuration.GetValue<string>("GatewaySettings:SecretTokenHeader");
            Program.secretToken = new StringValues(Configuration.GetValue<string>("Keys:SecretToken"));
            Program.databaseUpdatePassword = Configuration.GetValue<string>("Keys:DatabaseUpdatePassword");
            Program.languages = Configuration.GetSection("Languages").Get<Language[]>();
            Program.primaryDateSouceURL = Configuration.GetValue<string>("PrimaryDataSource:URL");

            Program.telemetry.TrackTrace("The app service has been initialized");
            Program.telemetry.TrackEvent("STARTUP");

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope();
            
            var context = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
            context.Database.EnsureCreated();

            app.UseHttpsRedirection();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "TC Legislation and Regulation API v1");
                c.DocumentTitle = "TC Legs & Regs API Definition";
                
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            SeedData.context = context;
        }
    }
}
