using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text.Extension;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using Newtonsoft.Json.Extension;
using WindNight.NetCore.Extension.Internals;

namespace WindNight.NetCore.Extension
{
    /// <summary>    </summary>
    public class CurrentItem : IDisposable
    {
        private static readonly object LockSerialNumber = new object();
        private static IDictionary<object, object> _items;

        /// <summary>
        ///     Gets or sets a key/value collection that can be used to share data within the scope of this request.
        ///     请勿在异步环境中使用此属性
        /// </summary>
        public static IDictionary<object, object> Items
        {
            get
            {
                try
                {
                    return Ioc.GetService<IHttpContextAccessor>()?.HttpContext?.Items;
                }
                catch
                {
                    return _items ?? (_items = new ConcurrentDictionary<object, object>());
                }
            }
        }

        /// <summary>  </summary>
        public static string GetSerialNumber
        {
            get
            {
                var ser = GetItem<string>(Common.SERIZLNUMBER);
                if (string.IsNullOrEmpty(ser))
                    lock (LockSerialNumber)
                    {
                        ser = GetItem<string>(Common.SERIZLNUMBER);
                        if (string.IsNullOrEmpty(ser))
                        {
                            ser = GuidHelper.GenerateOrderNumber();
                            AddItem(Common.SERIZLNUMBER, ser);
                        }
                    }

                return ser;
            }
        }

        /// <summary>    </summary>
        public void Dispose()
        {
            _items?.Clear();
        }

        /// <summary>    </summary>
        ~CurrentItem()
        {
            _items?.Clear();
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T GetItem<T>(string key)
        {
            var defaultValue = default(T);
            try
            {
                if (string.IsNullOrEmpty(key) || Items == null)
                    return defaultValue;
                if (!Items.ContainsKey(key))
                    return defaultValue;
                var value = Items[key];

                return value.To<T>();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return defaultValue;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void AddItem(string key, object value)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(key))
                    return;
                if (value == null || Items == null)
                    return;
                if (!Items.ContainsKey(key))
                    Items.Add(key, value);
                else
                    Items[key] = value;
            }
            catch
            {
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void AddItemIfNotExits(string key, object value)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(key))
                    return;
                if (value == null || Items == null)
                    return;
                if (!Items.ContainsKey(key))
                    Items.Add(key, value);
            }
            catch
            {
            }
        }
    }
}