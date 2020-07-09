using System;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using WindNight.NetCore.Core;
using WindNight.NetCore.Core.Abstractions;
using WindNight.NetCore.Core.Internals;

namespace WindNight.NetCore.Tools
{
    /// <summary>
    /// </summary>
    public static class TimeWatcherHelper
    {
        private const int DefaultWarnMiSeconds = 100;
        private static readonly string TimeWatcherIsOpenKey = "TimeWatcherIsOpen";

        private static bool TimeWatcherIsOpen
        {
            get
            {
                var configService = Ioc.GetService<IConfigService>();
                if (configService == null) return false;
                var configValue = configService.GetAppSetting(TimeWatcherIsOpenKey, "1", false);
                return configValue == "1";
            }
        }

        private static int GetWarnMiSeconds(int warnMiSeconds)
        {
            return warnMiSeconds > 0 ? warnMiSeconds : DefaultWarnMiSeconds;
        }


        /// <summary>
        ///     Safe ,not throw Exception
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <param name="watcherName"></param>
        /// <param name="appendMessage"></param>
        /// <param name="warnMiSeconds"></param>
        /// <returns></returns>
        public static T TimeWatcher<T>(Func<T> func, string watcherName = "", bool appendMessage = false,
            int warnMiSeconds = 200)
        {
            return DoWatcherFunc(func, watcherName, appendMessage, warnMiSeconds, false);
        }

        /// <summary>
        ///     Unsafe ,will throw Exception
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <param name="watcherName"></param>
        /// <param name="appendMessage"></param>
        /// <param name="warnMiSeconds"></param>
        /// <returns></returns>
        public static T TimeWatcherUnsafe<T>(Func<T> func, string watcherName = "", bool appendMessage = false,
            int warnMiSeconds = 200)
        {
            return DoWatcherFunc(func, watcherName, appendMessage, warnMiSeconds, true);
        }


        private static T DoWatcherFunc<T>(Func<T> func, string watcherName, bool appendMessage, int warnMiSeconds,
            bool isThrow)
        {
            T rlt;
            var ticks = DateTime.Now.Ticks;

            if (string.IsNullOrEmpty(watcherName)) watcherName = nameof(func);
            try
            {
                rlt = func.Invoke();
            }
            catch (BusinessException bex)
            {
                LogHelper.Warn($"TimeWatcher({watcherName}) 捕获业务异常：{bex.BusinessCode}", bex,
                    appendMessage: appendMessage);
                throw;
            }
            catch (Exception ex)
            {
                rlt = default;
                LogHelper.Error($"TimeWatcher({watcherName}) 捕获未知异常:{ex.GetType()}", ex, appendMessage: appendMessage);
                if (isThrow) throw;
            }
            finally
            {
                var milliseconds = (long) TimeSpan.FromTicks(DateTime.Now.Ticks - ticks).TotalMilliseconds;
                var fwarnMiS = GetWarnMiSeconds(warnMiSeconds);
                if (milliseconds > fwarnMiS)
                    LogHelper.Warn($"{watcherName} 耗时{milliseconds} ms ", appendMessage: appendMessage);
                if (milliseconds > 1 && TimeWatcherIsOpen)
                    LogHelper.Info($"{watcherName} 耗时{milliseconds} ms ");
            }

            return rlt;
        }

        /// <summary>
        ///     Safe ,not throw Exception
        /// </summary>
        /// <param name="action"></param>
        /// <param name="watcherName"></param>
        /// <param name="appendMessage"></param>
        /// <param name="warnMiSeconds"></param>
        public static void TimeWatcher(Action action, string watcherName = "", bool appendMessage = false,
            int warnMiSeconds = 200)
        {
            DoWatcherAction(action, watcherName, appendMessage, warnMiSeconds, false);
        }

        /// <summary>
        ///     will throw Exception
        /// </summary>
        /// <param name="action"></param>
        /// <param name="watcherName"></param>
        /// <param name="appendMessage"></param>
        /// <param name="warnMiSeconds"></param>
        public static void TimeWatcherUnsafe(Action action, string watcherName = "", bool appendMessage = false,
            int warnMiSeconds = 200)
        {
            DoWatcherAction(action, watcherName, appendMessage, warnMiSeconds, true);
        }

        private static void DoWatcherAction(Action action, string watcherName, bool appendMessage, int warnMiSeconds,
            bool isThrow)
        {
            var ticks = DateTime.Now.Ticks;
            try
            {
                action.Invoke();
            }
            catch (BusinessException bex)
            {
                LogHelper.Warn($"TimeWatcher({watcherName}) 捕获业务异常：{bex.BusinessCode}", bex,
                    appendMessage: appendMessage);
                throw;
            }
            catch (Exception ex)
            {
                LogHelper.Error($"TimeWatcher({watcherName}) 捕获未知异常:{ex.GetType()}", ex, appendMessage: appendMessage);
                if (isThrow) throw;
            }
            finally
            {
                if (string.IsNullOrEmpty(watcherName)) watcherName = nameof(action);
                var milliseconds = (long) TimeSpan.FromTicks(DateTime.Now.Ticks - ticks).TotalMilliseconds;
                var fwarnMiS = GetWarnMiSeconds(warnMiSeconds);
                if (milliseconds > fwarnMiS)
                    LogHelper.Warn($"{watcherName} 耗时{milliseconds} ms ", appendMessage: appendMessage);
                if (milliseconds > 1 && TimeWatcherIsOpen)
                    LogHelper.Info($"{watcherName} 耗时{milliseconds} ms ");
            }
        }
    }
}