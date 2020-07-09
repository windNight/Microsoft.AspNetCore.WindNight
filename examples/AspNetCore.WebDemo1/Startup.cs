using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.WnExtensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.WnExtensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using WindNight.NetCore.Core.Abstractions;
using IpHelper = WindNight.NetCore.Extension.HttpContextExtension;

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
