using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Photon.Webhooks.Turnbased.Config;
using Photon.Webhooks.Turnbased.DataAccess;
using Photon.Webhooks.Turnbased.PushNotifications;

namespace Photon.Webhooks.Turnbased
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.Configure<ConnectionStrings>(Configuration.GetSection("connectionStrings"));
            services.Configure<AppSettings>(Configuration.GetSection("appSettings"));
            services.AddLogging();
            services.AddTransient<INotification, AzureHubNotification>();
            services.AddTransient<DataSources>();
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddAzureWebAppDiagnostics();
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "ApiByAppId",
                    template: "{appId}/{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
