using System;
using System.Runtime.Caching;

namespace Infonet.CStoreCommander.BusinessLayer.Utilities
{
    public class MemoryCacher
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object GetValue(string key)
        {
            var memoryCache = MemoryCache.Default;
            return memoryCache.Get(key);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="absExpiration"></param>
        /// <returns></returns>
        public bool Add(string key, object value, DateTimeOffset absExpiration)
        {
            var memoryCache = MemoryCache.Default;
            return memoryCache.Add(key, value, absExpiration);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        public void Delete(string key)
        {
            var memoryCache = MemoryCache.Default;
            if (memoryCache.Contains(key))
            {
                memoryCache.Remove(key);
            }
        }

    }//end class
}//end namespace
