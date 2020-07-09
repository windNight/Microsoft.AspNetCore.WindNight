#if NETSTANDARD
using Microsoft.Extensions.DependencyInjection.WnExtension;
using System.Collections.Generic;

namespace Microsoft.Extensions.Configuration.Extensions
{
    /// <summary>
    /// </summary>
    public static class ConfigurationExtension
    {
        /// <summary>
        /// </summary>
        /// <param name="sections"></param>
        /// <returns></returns>
        public static object GetConfiguration(IEnumerable<IConfigurationSection> sections = null)
        {
            var _config = Ioc.GetService<IConfiguration>();
            if (sections == null) sections = _config.GetChildren();

            var list = new List<object>();
            foreach (var m in sections)
            {
                if (m.Value != null)
                {
                    list.Add(new { m.Key, m.Path, m.Value });
                    continue;
                }

                var _section = _config.GetSection(m.Path);
                list.Add(new { m.Key, m.Path, Value = GetConfiguration(_section.GetChildren()) });
            }

            return list;
        }

        /// <summary>
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="sections"></param>
        /// <returns></returns>
        public static object GetConfiguration(this IConfiguration configuration,
            IEnumerable<IConfigurationSection> sections = null)
        {
            var _config = configuration;
            if (sections == null) sections = _config.GetChildren();

            var list = new List<object>();
            foreach (var m in sections)
            {
                if (m.Value != null)
                {
                    list.Add(new { m.Key, m.Path, m.Value });
                    continue;
                }

                var _section = _config.GetSection(m.Path);
                list.Add(new { m.Key, m.Path, Value = _config.GetConfiguration(_section.GetChildren()) });
            }

            return list;
        }
    }
}
#endif