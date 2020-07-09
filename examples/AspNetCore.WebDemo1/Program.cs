using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.WnExtensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;

namespace AspNetCore.WebDemo1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var buildType = "";
#if DEBUG
            buildType = "Debug";
#else
            buildType = "Release";
#endif
            ProgramBase.Init(CreateHostBuilder, buildType, args);
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return ProgramBase.CreateHostBuilderDefaults(args,
                configBuilder =>
                {
                    configBuilder.SetBasePath(AppContext.BaseDirectory)
                        .AddJsonFile("appsettings.json", false, true)
                        ;
                },
                webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
        }


    }
}
