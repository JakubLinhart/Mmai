using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mmai.Models;
using System.Collections.Generic;
using System.Globalization;

namespace Mmai
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddLocalization(x => x.ResourcesPath = "Resources");

            services.AddSingleton<IGameEventRepository, GameEventRepository>();
            services.AddSingleton<IGameRepository, GameRepository>();
            services.AddSingleton<ISpeciesRepository, SpeciesRepository>();
            services.AddSingleton<IGameStatisticsRepository, GameStatisticsRepository>();

            var supportedCultures = new List<CultureInfo>
                {
                    new CultureInfo("cs"),
                    new CultureInfo("en"),
                    new CultureInfo("pl"),
                };

            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseRequestLocalization();


            app.UseRequestLocalization();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id=littleowl}");
            });
        }
    }
}
