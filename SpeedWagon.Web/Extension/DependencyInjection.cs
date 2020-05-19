using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SpeedWagon.Interfaces;
using SpeedWagon.Runtime.Interfaces;
using SpeedWagon.Runtime.Services;
using SpeedWagon.Services;
using SpeedWagon.Web.Auth;
using SpeedWagon.Web.Enum;
using SpeedWagon.Web.Interfaces;
using SpeedWagon.Web.Services;
using System.IO;

namespace SpeedWagon.Web.Extension
{
    public static class DependencyInjection
    {
        private  const string SPEEDWAGON_HOST = "https://reo.speedwagon.me";
        
        public static IServiceCollection AddSpeedWagonCms(this IServiceCollection services, string path, string uploadsPath, IFileProvider contentFileProvider, IFileProvider uploadFileProvider)
        {

            IContentService contentService = new CacheLessRuntimeContentService(path, null, contentFileProvider);
            IEditorService editorService = new EditorService(contentService, SPEEDWAGON_HOST);
            IContentTypeService contentTypeService = new ContentTypeService(contentService, SPEEDWAGON_HOST);
            IWebContentService webContentService = new WebContentService(contentService, SPEEDWAGON_HOST);

            IContentService uploadContentService = new CacheLessRuntimeContentService(uploadsPath, null, uploadFileProvider);
            IFileUploadService fileUploadService = new FileUploadService(uploadContentService, string.Empty, uploadFileProvider);
            ISearchService searchService = new LuceneSearchService(contentService, Path.Combine(path, "search"));

            services.AddSingleton<ISpeedWagonAdminContext>(s => 
                new SpeedWagonAdminContext(
                    path, 
                    contentService,
                    contentTypeService,
                    editorService, 
                    webContentService,
                    fileUploadService,
                    searchService)
            );
         
            return services;
        }

        

        public static IServiceCollection AddSpeedWagon(this IServiceCollection services, string path, bool cached, IFileProvider contentFileProvider)
        {

            IContentService contentService;
            if (cached)
            {
                services.AddHostedService<CacheRefreshingHostedService>();
                contentService = new CachedRuntimeContentService(
                            path, null, contentFileProvider
                         );

                
            } else
            {
                contentService = new CacheLessRuntimeContentService(
                             path, null, contentFileProvider
                         );
            }

            ISearchService searchService = new LuceneSearchService(contentService, Path.Combine(path, "search"));
            services.AddSingleton<ISpeedWagonWebContext>(
                    s => new SpeedWagonWebContext(
                        path, contentService, searchService));

            return services;
        }

        public static IServiceCollection AddAzureAdAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IAuthTypeInformationProvider>(s => new AuthTypeInformationProvider(AuthType.AzureAd));

            services.AddAuthentication(sharedOptions =>
            {
                sharedOptions.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                sharedOptions.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            // Use Azure AD Login
            .AddAzureAd(options => configuration.Bind("AzureAd", options))
            .AddCookie();
            
            return services;
        }

        public static IServiceCollection AddSimpleAuthentication(this IServiceCollection services)
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

            return services;
        }
    }
}
