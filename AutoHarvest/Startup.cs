using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AutoHarvest.Singleton;
using AspNetCoreRateLimit;
using AutoHarvest.Scrapers;

namespace AutoHarvest
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
            services.AddRazorPages();

            // add all the web scrapers
            services.AddSingleton<Carsales>();
            services.AddSingleton<FbMarketplace>();
            services.AddSingleton<Gumtree>();

            // add the scraper wrapper class
            services.AddSingleton<CarWrapper>();

            // add the headless browser
            services.AddSingleton<CefSharpHeadless>();

            // add the LogoLookup class
            services.AddSingleton<LogoLookup>();

            // add the event class
            services.AddSingleton<Events>();

            services.AddMemoryCache();

            // add a rate limit so users can't send to many requests
            var rateLimitRules = new List<RateLimitRule>
            {
                new RateLimitRule
                {
                    Endpoint = "*:/",
                    Limit = 3,
                    Period = "1s"
                }
            };
            services.Configure<IpRateLimitOptions>(opt =>
            {
                opt.EnableEndpointRateLimiting = true;
                opt.GeneralRules = rateLimitRules;
                opt.QuotaExceededResponse = new QuotaExceededResponse
                {
                    Content = "{{ \"message\": \"Whoa! Calm down, cowboy!\", \"details\": \"Quota exceeded. Maximum allowed: {0} per {1}. Please try again in {2} second(s).\" }}",
                    ContentType = "application/json",
                    StatusCode = 429
                };
            });
            services.AddInMemoryRateLimiting();

            // configuration (resolvers, counter key builders)
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
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
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseResponseCaching();
            app.UseIpRateLimiting();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
