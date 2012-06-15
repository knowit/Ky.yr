using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web;
using System.Runtime.Caching;
using System.Xml.Serialization;

namespace Ky.yr
{
    public class WeatherHandler
    {
        private static WebClient _client;
        private static ObjectCache _cache;

        private WeatherHandler()
        {
            _client = _client ?? new WebClient();
            _client.Encoding = Encoding.UTF8;
            _cache = _cache ?? MemoryCache.Default;
            

        }
        private string GetData(string url)
        {
            var result = string.Empty;

            try
            {

                // try getting the item from the short term cache
                result = (string) _cache.Get(url);

                // if short term cache does not return a result for this uri, load it from finn.no
                if (string.IsNullOrEmpty(result))
                {
                    result = _client.DownloadString(url);

                    // new content was downloaded, update the local cache stores for this item.
                    _cache.Set(url, result, new DateTimeOffset(DateTime.Now.AddMinutes(15)));

                    // add a long time cache store, to use in case of service outage from finn.no
                    _cache.Set("lt:" + url, result, new DateTimeOffset(DateTime.Now.AddDays(1)));
                }
            }
            catch (Exception e)
            {
                // if the web client fails to download content from finn.no try looking in the long time cache store to serve content to the user during the outage
                result = _cache.Get("lt:" + url) as string ?? string.Empty;
            }
            return result;
        }
        public static DynamicXml GetForeceast(string url)
        {
            WeatherHandler _me = new WeatherHandler();
            string result = _me.GetData(url);
            return new DynamicXml(result);
        }
        
    }
}