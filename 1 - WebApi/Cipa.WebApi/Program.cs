using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Cipa.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    #if DEBUG
                    config.AddSystemsManager("/Cipa/Dev");
                    #else
                    config.AddSystemsManager("/Cipa/");
                    #endif
                })
                .UseStartup<Startup>();
    }
}
