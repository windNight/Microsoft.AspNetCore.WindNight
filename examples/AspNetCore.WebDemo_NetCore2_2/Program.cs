using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;

namespace AspNetCore.WebDemo_NetCore2_2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                          .ConfigureAppConfiguration(configBuilder =>
                          {
                              configBuilder.SetBasePath(AppContext.BaseDirectory)
                                  .AddJsonFile("appsettings.json", false, true)
                                  ;
                          })
                          .UseStartup<Startup>();
        }



    }
}
