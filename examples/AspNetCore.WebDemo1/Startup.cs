using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.WnExtensions;
using Microsoft.AspNetCore.Mvc.WnExtensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using WindNight.Core.Abstractions;

namespace AspNetCore.WebDemo1
{
    public class Startup : WebStartupBase
    {
        public Startup(IConfiguration configuration) : base(configuration)
        {
        }
        protected override string NamespaceName => Assembly.GetEntryAssembly()?.FullName;

        protected override void UseBizConfigure(IApplicationBuilder app)
        {
        }

        protected override void ConfigBizServices(IServiceCollection services)
        {
            services.AddSingleton<ILogService, DefaultLogService>();
            services.AddSingleton<IConfigService, DefaultConfigService>();
        }
    }




}
