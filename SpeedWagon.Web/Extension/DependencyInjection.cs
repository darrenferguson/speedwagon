using Microsoft.Extensions.DependencyInjection;
using SpeedWagon.Interfaces;
using SpeedWagon.Runtime.Interfaces;
using SpeedWagon.Runtime.Services;
using SpeedWagon.Services;
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
            

            if (cached)
            {
                services.AddHostedService<CacheRefreshingHostedService>();

                services.AddSingleton<ISpeedWagonWebContext>(s => new SpeedWagonWebContext(path, new CachedRuntimeContentService(path, null, contentFileProvider)));
            } else
            {
                services.AddSingleton<ISpeedWagonWebContext>(s => new SpeedWagonWebContext(path, new CacheLessRuntimeContentService(path, null, contentFileProvider)));
            }

            return services;
        }
    }
}
