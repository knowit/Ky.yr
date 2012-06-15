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
    /// <summary>
    /// Connects to the URL specified, downloads the content as a string and caches the content for 15 and 24 hours.
    /// Consecutive calls to this method will check cache first and if the Url specified exist in the cache it will get content from cache instead of getting it from the URL specified.
    /// If the target URL is unreachable, it will try to provide data from the 24 hour cache to serve data.
    /// </summary>
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
                // if the web client fails to download content from target URL, try looking in the long time cache store to serve content to the user during the outage
                result = _cache.Get("lt:" + url) as string ?? string.Empty;
            }
            return result;
        }
        /// <summary>
        /// Instansiate a cache object for the given URL. Data in the cache will be stored for 15 minutes and 24 hours.
        /// As long as data exist for the given URL in the 15 minute cache, it will not be updated from target.
        /// If target is unavailable, cache will try to find data from the 24 hour cache store instead to serve content.
        /// </summary>
        /// <param name="url">URL to yr.no weather xml file. E.g: http://www.yr.no/place/Norway/Hordaland/Bergen/Bergen/forecast.xml </param>
        /// <returns>DynamicXml object or null if data could not be retrieved</returns>
        public static DynamicXml GetForeceast(string url)
        {
            WeatherHandler _me = new WeatherHandler();
            string result = _me.GetData(url);
            return (!string.IsNullOrEmpty(result)) ? new DynamicXml(result) : null;
        }
        
    }
}