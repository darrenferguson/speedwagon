using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using SpeedWagon.Runtime.Services.Files;
using SpeedWagon.Web.Extension;
using System.IO;
using SpeedWagon.Runtime.Interfaces;

namespace SpeedWagon.New
{
    public class Startup
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<Startup> _logger;
        private IConfiguration _configuration;

        public Startup(IConfiguration configuration, 
            IWebHostEnvironment env, 
            ILogger<Startup> logger)
        {
            this._configuration = configuration;
            this._env = env;
            this._logger = logger;
        }

        public void ConfigureServices(IServiceCollection services)
        {

            // Simple Authentication = No Authentication!!
            //if (this._configuration["SpeedWagon:Login"] == "Simple")
            //{
                services.AddSimpleAuthentication();
            //}
            //else
            //{
            //    services.AddAzureAdAuthentication(this._configuration);
            //}

            string path = Path.Combine(this._env.WebRootPath, "content");
           
            // Proivders for file storage.
            IFileProvider contentFileProvider = new FileSystemFileProvider();
            IFileProvider uploadFileProvider;

            //if (this._configuration["Files:Provider"] == "Blob")
            //{
            //    // Container must exist
            //    uploadFileProvider = new BlobFileProvider(this._configuration["Files:ConnectionString"], "speedwagon", this._env.WebRootPath);
            //}
            //else
            //{
                uploadFileProvider = new FileSystemFileProvider();
            //}
            
            // Add Front end
            bool.TryParse(this._configuration["SpeedWagon:CacheRuntime"], out bool cahceRuntime);
            services.AddSpeedWagon(path, cahceRuntime, contentFileProvider);

            // Optionally register CMS backend
            //bool.TryParse(this._configuration["SpeedWagon:RegisterCms"], out bool registerCms);
            //if (registerCms)
            //{
                services.AddSpeedWagonCms(path, this._env.WebRootPath, contentFileProvider, uploadFileProvider);
            //}

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
                    new Microsoft.Extensions.FileProviders.PhysicalFileProvider(Path.Combine(env.WebRootPath, "speedwagon"))
            });

            app.UseStaticFiles(new StaticFileOptions
            {
                RequestPath = new PathString("/theme"),
                FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(Path.Combine(env.WebRootPath, "theme"))
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