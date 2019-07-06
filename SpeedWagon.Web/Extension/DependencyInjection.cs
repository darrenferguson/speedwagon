using Microsoft.Extensions.DependencyInjection;
using SpeedWagon.Interfaces;
using SpeedWagon.Runtime.Services.Files;
using SpeedWagon.Services;
using SpeedWagon.Web.Interfaces;
using SpeedWagon.Web.Services;

namespace SpeedWagon.Web.Extension
{
    public static class DependencyInjection
    {
        private  const string SPEEDWAGON_HOST = "https://reo.speedwagon.me";

        public static IServiceCollection AddSpeedWagonCms(this IServiceCollection services, string path)
        {

            IContentService contentService = new CacheLessRuntimeContentService(path, null, new FileSystemFileProvider());
            IEditorService editorService = new EditorService(contentService, SPEEDWAGON_HOST);
            IContentTypeService contentTypeService = new ContentTypeService(contentService, SPEEDWAGON_HOST);
            IWebContentService webContentService = new WebContentService(contentService, SPEEDWAGON_HOST);

            services.AddSingleton<ISpeedWagonAdminContext>(s => 
                new SpeedWagonAdminContext(path, contentService, contentTypeService, editorService, webContentService)
            );
            
            return services;
        }

        public static IServiceCollection AddSpeedWagon(this IServiceCollection services, string path, bool cached)
        {

            if (cached)
            {
                services.AddSingleton<ISpeedWagonWebContext>(s => new SpeedWagonWebContext(path, new CachedRuntimeContentService(path, null, new FileSystemFileProvider())));
            } else
            {
                services.AddSingleton<ISpeedWagonWebContext>(s => new SpeedWagonWebContext(path, new CacheLessRuntimeContentService(path, null, new FileSystemFileProvider())));
            }
            return services;
        }
    }
}
