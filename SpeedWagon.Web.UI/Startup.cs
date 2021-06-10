using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using SpeedWagon.Runtime.Services.Files;
using SpeedWagon.Web.Extension;
using SpeedWagon.Web.Services;
using System.IO;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;

namespace SpeedWagon.Web.UI
{
    public class Startup
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<Startup> _logger;

        public Startup(IConfiguration configuration, IWebHostEnvironment env, ILogger<Startup> logger)
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

            // Simple Authentication = No Authentication.
            if (auth == "Simple")
            {
                services.AddSimpleAuthentication();
            }
            else
            {
                services.AddAzureAdAuthentication(Configuration);
            }

            this._logger.LogInformation("Initialised auth");

            // Start with / for relative - or specify an absolute path
            string path;
            string contentPath = Configuration["SpeedWagon:ContentPath"];
            if (contentPath.StartsWith("/"))
            {
                path = Path.Combine(this._env.ContentRootPath, _appDataFolder, contentPath.Substring(1));
            } else
            {
                path = contentPath;
            }

            string uploadPath = Path.Combine(this._env.ContentRootPath, "wwwroot");

            // Proivders for file storage.
            Runtime.Interfaces.IFileProvider contentFileProvider = new FileSystemFileProvider();
            Runtime.Interfaces.IFileProvider uploadFileProvider;

            string fileProvider = Configuration["Files:Provider"];
            if (fileProvider == "Blob")
            {
                // Container must exist
                uploadFileProvider = new BlobFileProvider(Configuration["Files:ConnectionString"], "speedwagon", uploadPath);
            }
            else
            {
                uploadFileProvider = new FileSystemFileProvider();
            }
            
            // Add Front end
            bool.TryParse(Configuration["SpeedWagon:CacheRuntime"], out bool cahceRuntime);
            services.AddSpeedWagon(path, cahceRuntime, contentFileProvider);

            // Optionally register CMS backend
            bool.TryParse(Configuration["SpeedWagon:RegisterCms"], out bool registerCms);
            if (registerCms)
            {
                services.AddSpeedWagonCms(path, uploadPath, contentFileProvider, uploadFileProvider);
            }

            services.AddMvc();
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
            }

            app.UseStaticFiles(new StaticFileOptions
            {
                RequestPath = new PathString("/speedwagon"),
                FileProvider =
                    new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "wwwroot", "speedwagon"))
            });

           

            app.UseStaticFiles(new StaticFileOptions
            {
                RequestPath = new PathString("/theme"),
                FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "wwwroot", "theme"))
            });

            app.UseStaticFiles(new StaticFileOptions
            {
                RequestPath = new PathString("/webfonts"),
                FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "wwwroot", "webfonts"))
            });

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
        
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                
                endpoints.MapControllerRoute(
                    "default", "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                   "404-PageNotFound", "{*url}",
                    new { controller = "Home", action = "Index" });
            });
        }
    }
}