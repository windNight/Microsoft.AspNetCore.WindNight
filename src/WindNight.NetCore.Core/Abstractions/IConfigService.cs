﻿namespace WindNight.NetCore.Core.Abstractions
{
    /// <summary>
    /// </summary>
    public interface IConfigService
    {
        /// <summary>
        /// </summary>
        /// <param name="connKey"></param>
        /// <param name="defaultValue"></param>
        /// <param name="isThrow"></param>
        /// <returns></returns>
        string GetConnString(string connKey, string defaultValue = "", bool isThrow = true);


        /// <summary>
        /// </summary>
        /// <param name="configKey"></param>
        /// <param name="defaultValue"></param>
        /// <param name="isThrow"></param>
        /// <returns></returns>
        string GetAppSetting(string configKey, string defaultValue = "", bool isThrow = true);

        /// <summary>
        /// </summary>
        /// <param name="configKey"></param>
        /// <param name="defaultValue"></param>
        /// <param name="isThrow"></param>
        /// <returns></returns>
        int GetAppSetting(string configKey, int defaultValue = 0, bool isThrow = true);

        /// <summary>
        /// </summary>
        /// <param name="configKey"></param>
        /// <param name="defaultValue"></param>
        /// <param name="isThrow"></param>
        /// <returns></returns>
        bool GetAppSetting(string configKey, bool defaultValue = false, bool isThrow = true);


        /// <summary>
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="defaultValue"></param>
        /// <param name="isThrow"></param>
        /// <returns></returns>
        string GetFileConfigString(string fileName, string defaultValue = "", bool isThrow = true);

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <param name="isThrow"></param>
        /// <returns></returns>
        T GetFileConfig<T>(string fileName, bool isThrow = true) where T : new();
    }
}