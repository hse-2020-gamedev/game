using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace FrontendServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseUrls("https://*:6014", "http://*:6013");
                    webBuilder.UseStartup<Startup>();
                });
    }
}
