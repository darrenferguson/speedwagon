using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SpeedWagon.Web;
using SpeedWagon.Web.Interfaces;

namespace SpeedWagon.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            using (ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider())
            {
                ISpeedWagonAdminContext context = serviceProvider.GetService<ISpeedWagonAdminContext>();
                context.Install("test@test.com");
            }
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            //services.AddSingleton<IContentService>(
            //   s => new CacheLessRuntimeContentService(@"d:\reo", new[] { "moriyama.co.uk" }));

            //services.AddSingleton<IContentService>(
            //   s => new CachedRuntimeContentService(@"d:\reo", null));

            string path = @"d:\reo";

            services.AddSingleton<ISpeedWagonAdminContext>(
              s => new SpeedWagonAdminContext(path));

            services.AddLogging(loggingBuilder => loggingBuilder
             .ClearProviders()
             .AddConsole()
             .AddDebug()
             .SetMinimumLevel(LogLevel.Debug));

            //var configuration = new ConfigurationBuilder()
            //   .SetBasePath(AppContext.BaseDirectory)
            //   .AddJsonFile("appsettings.json", false)
            //   .Build();
            
        }
    }
}