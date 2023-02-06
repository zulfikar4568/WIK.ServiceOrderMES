using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;
using System.Text.Json;
using WIK.ServiceOrderMES.Config;
using WIK.ServiceOrderMES.Util;
using System.Reflection;

namespace WIK.ServiceOrderMES.Driver
{
    public static class Redis
    {
        public static async Task SetRecordAsync<T>(string recordId, T data, TimeSpan? absoluteExpTime = null)
        {
            try
            {
                var jsonData = JsonSerializer.Serialize(data);
                await redis.GetDatabase().StringSetAsync(recordId, jsonData, absoluteExpTime ?? TimeSpan.FromSeconds(60));
            }
            catch (Exception ex)
            {
                ex.Source = AppSettings.AssemblyName == ex.Source ? MethodBase.GetCurrentMethod().Name : MethodBase.GetCurrentMethod().Name + "." + ex.Source;
                EventLogUtil.LogErrorEvent(ex.Source, ex);
            }
        }

        public static async Task<Boolean> DeleteRecordAsync(string recordId)
        {
            try
            {
                await redis.GetDatabase().KeyDeleteAsync(recordId);
                return false;
            }
            catch (Exception ex)
            {
                ex.Source = AppSettings.AssemblyName == ex.Source ? MethodBase.GetCurrentMethod().Name : MethodBase.GetCurrentMethod().Name + "." + ex.Source;
                EventLogUtil.LogErrorEvent(ex.Source, ex);
                return false;
            }
        }

        public static async Task<T> GetRecordAsync<T>(string recordId)
        {
            try
            {
                RedisValue jsonData = await redis.GetDatabase().StringGetAsync(recordId);
                if (jsonData.IsNullOrEmpty)
                {
                    return default;
                }

                return JsonSerializer.Deserialize<T>(jsonData);
            }
            catch (Exception ex)
            {
                ex.Source = AppSettings.AssemblyName == ex.Source ? MethodBase.GetCurrentMethod().Name : MethodBase.GetCurrentMethod().Name + "." + ex.Source;
                EventLogUtil.LogErrorEvent(ex.Source, ex);
                return default;
            }
        }

        public static async Task<Boolean> DeleteRecordKeysPattern(string pattern)
        {
            try
            {
                var keys = await Task.Run(() => redis.GetServer(AppSettings.CachedHost).Keys(pattern: pattern));
                foreach (var key in keys)
                {
                    await redis.GetDatabase().KeyDeleteAsync(key);
                }
                return true;

            }
            catch (Exception ex)
            {
                ex.Source = AppSettings.AssemblyName == ex.Source ? MethodBase.GetCurrentMethod().Name : MethodBase.GetCurrentMethod().Name + "." + ex.Source;
                EventLogUtil.LogErrorEvent(ex.Source, ex);
                return false;
            }
        }

        public static async Task<List<string>> GetRecordKeysPattern(string pattern)
        {
            try
            {
                List<string> keyCollections = new List<string>();
                var keys = await Task.Run(() => redis.GetServer(AppSettings.CachedHost).Keys(pattern: pattern));
                foreach (var key in keys)
                {
                    keyCollections.Add(key.ToString());
                }
                return keyCollections;
            }
            catch (Exception ex)
            {
                ex.Source = AppSettings.AssemblyName == ex.Source ? MethodBase.GetCurrentMethod().Name : MethodBase.GetCurrentMethod().Name + "." + ex.Source;
                EventLogUtil.LogErrorEvent(ex.Source, ex);
                return default;
            }
        }

        public static readonly ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(new ConfigurationOptions { EndPoints = { AppSettings.CachedHost } });
    }
}
