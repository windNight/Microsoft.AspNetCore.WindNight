using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.WnExtensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using Swashbuckle.AspNetCore.Extensions;
using System;
using System.Reflection;
using WindNight.Core.Abstractions;

namespace AspNetCore.WebDemo_NetCore2_2
{
    public class Startup
    {
        protected static string NamespaceName => Assembly.GetEntryAssembly()?.FullName;

        protected void UseBizConfigure(IApplicationBuilder app)
        {

        }

        protected void ConfigBizServices(IServiceCollection services)
        {
            services.AddSingleton<ILogService, DefaultLogService>();
            services.AddSingleton<IConfigService, DefaultConfigService>();
        }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected IConfiguration Configuration { get; }

        public IServiceProvider BuildServices(IServiceCollection services)
        {
            var serviceProvider = CreateServiceProvider(services);
            Ioc.Instance.InitServiceProvider(serviceProvider);
            return serviceProvider;
        }

        public virtual void ConfigureServices(IServiceCollection services)
        {
            ConfigSysServices(services, Configuration);
            ConfigBizServices(services);
            BuildServices(services);
            services.AddCors(options =>
            {
                options.AddPolicy("any", builder =>
                {
                    builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                });
            });
        }

        public virtual void Configure(IApplicationBuilder app)
        {
            UseBizConfigure(app);
            UseSysConfigure(app);
        }

        public virtual IServiceProvider CreateServiceProvider(
            IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            return serviceProvider;
        }


        protected virtual void UseSysConfigure(IApplicationBuilder app)
        {
            app.UseSwaggerConfig(NamespaceName);
            app.UseStaticFiles(new StaticFileOptions
            {
                ServeUnknownFileTypes = true,
            }); //启用静态资源

            // app.UseCors("any");
            app.UseCors(options => options.SetIsOriginAllowed(x => _ = true).AllowAnyMethod().AllowAnyHeader().AllowCredentials());
            app.UseMvc();
            app.UseStaticFiles();



        }

        protected virtual IServiceCollection ConfigSysServices(IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; });
            services.AddMvcBuilderWithDefaultFilters();

            services.AddSwaggerConfig(NamespaceName, configuration);
            return services;
        }
    }
}
