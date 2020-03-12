using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SpeedWagon.Runtime.Interfaces;
using SpeedWagon.Runtime.Services.Files;
using SpeedWagon.Web.Auth;
using SpeedWagon.Web.Enum;
using SpeedWagon.Web.Extension;
using SpeedWagon.Web.Interfaces;
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

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            this._logger.LogInformation("Speedwagon is starting");

            string auth = Configuration["SpeedWagon:Login"];
            this._logger.LogInformation("Got config " + auth);

            if (auth != "Simple")
            {
                this._logger.LogInformation("Registering AD AUTH");
                services.AddSingleton<IAuthTypeInformationProvider>(s => new AuthTypeInformationProvider(AuthType.AzureAd));

                services.AddAuthentication(sharedOptions =>
                {
                    sharedOptions.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    sharedOptions.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                })
                // Use Azure AD Login
                .AddAzureAd(options => Configuration.Bind("AzureAd", options))
                .AddCookie();
                this._logger.LogInformation("Registered AD AUTH");
            }
            else
            {
                this._logger.LogInformation("Registering Simple AUTH");
                services.AddSingleton<IAuthTypeInformationProvider>(s => new AuthTypeInformationProvider(AuthType.Dummy));

                // Dummy Login provider
                services.AddAuthentication(sharedOptions =>
                {
                    sharedOptions.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme,
                    options =>
                    {
                        options.LoginPath = "/SpeedWagonAccount/Login";
                        options.LogoutPath = "/SpeedWagonAccount/Logout";
                    });

                services.AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                });
            }

            this._logger.LogInformation("Initialised auth");

            string path = Path.Combine(this._env.ContentRootPath, _appDataFolder, "speedwagon");
            string uploadPath = Path.Combine(this._env.ContentRootPath, "wwwroot");

            // COntent file provider can also use blob.
            //IFileProvider contentFileProvider = new BlobFileProvider("<connectionString>", "speedwagon");


            string blobConnection = Configuration["Blob:ConnectionString"];
            IFileProvider contentFileProvider = new FileSystemFileProvider();
            // IFileProvider uploadFileProvider = new BlobFileProvider(blobConnection, "speedwagon");

            services.AddSpeedWagon(path, false, contentFileProvider);
            services.AddSpeedWagonCms(path, uploadPath, contentFileProvider, contentFileProvider);

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
