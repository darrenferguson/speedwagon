using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SpeedWagon.Runtime.Interfaces;
using SpeedWagon.Runtime.Services.Files;
using SpeedWagon.Web.Extension;
using SpeedWagon.Web.Services;
using System.IO;

namespace SpeedWagon.Web.UI
{
    public class Startup
    {
        private readonly IHostingEnvironment _env;
        private readonly ILogger<Startup> _logger;
        public Startup(IConfiguration configuration, IHostingEnvironment env, ILogger<Startup> logger)
        {
            Configuration = configuration;
            this._env = env;
            this._logger = logger;
        }

        public IConfiguration Configuration { get; }

        private const string _appDataFolder = "AppData";

        public void ConfigureServices(IServiceCollection services)
        {
            this._logger.LogInformation("Speedwagon is starting");

            string auth = Configuration["SpeedWagon:Login"];
            this._logger.LogInformation("Got config " + auth);

            if (auth == "Simple")
            {
                services.AddSimpleAuthentication();
            }
            else
            {
                services.AddAzureAdAuthentication(Configuration);
            }

            this._logger.LogInformation("Initialised auth");

            string path = Path.Combine(this._env.ContentRootPath, _appDataFolder, "speedwagon");
            string uploadPath = Path.Combine(this._env.ContentRootPath, "wwwroot");

            // COntent file provider can also use blob.
            //IFileProvider contentFileProvider = new BlobFileProvider("<connectionString>", "speedwagon");

            IFileProvider contentFileProvider;
            IFileProvider uploadFileProvider;

            string fileProvider = Configuration["Files:Provider"];
            if(fileProvider == "Blob")
            {
                string blobConnection = Configuration["Files:ConnectionString"];
                contentFileProvider = new FileSystemFileProvider();
                uploadFileProvider = new BlobFileProvider(blobConnection, "speedwagon");
            } else
            {
                contentFileProvider = new FileSystemFileProvider();
                uploadFileProvider = new FileSystemFileProvider();
            }
                     
            services.AddSpeedWagon(path, false, contentFileProvider);
            services.AddSpeedWagonCms(path, uploadPath, contentFileProvider, uploadFileProvider);
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // app.UseDeveloperExceptionPage();
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                "404-PageNotFound",
                "{*url}",
                new { controller = "Home", action = "Index" }
                );
            });
        }
    }
}