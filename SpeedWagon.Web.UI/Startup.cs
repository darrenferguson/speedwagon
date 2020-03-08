using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            this._env = env;

        }

        public IConfiguration Configuration { get; }

        private const string _appDataFolder = "AppData";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string auth = Configuration["SpeedWagon:Login"];
            

            if (auth == "AzureAd")
            {
                services.AddSingleton<IAuthTypeInformationProvider>(s => new AuthTypeInformationProvider(AuthType.AzureAd));

                services.AddAuthentication(sharedOptions =>
                {
                    sharedOptions.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    sharedOptions.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                })
                // Use Azure AD Login
                .AddAzureAd(options => Configuration.Bind("AzureAd", options))
                .AddCookie();

            }
            else
            {
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

            string path = Path.Combine(this._env.ContentRootPath, _appDataFolder, "speedwagon");
            string uploadPath = Path.Combine(this._env.ContentRootPath, "wwwroot");

            // COntent file provider can also use blob.
            //IFileProvider contentFileProvider = new BlobFileProvider("<connectionString>", "speedwagon");


            string blobConnection = Configuration["Blob:ConnectionString"];
            IFileProvider contentFileProvider = new FileSystemFileProvider();
            // IFileProvider uploadFileProvider = new BlobFileProvider(blobConnection, "speedwagon");

            services.AddSpeedWagon(path, true, contentFileProvider);
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
            });
        }
    }
}
