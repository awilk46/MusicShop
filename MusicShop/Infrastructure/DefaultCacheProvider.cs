using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;

namespace MusicShop.Infrastructure
{
    //główna klasa do ustawiania keszaa
    public class DefaultCacheProvider : ICacheProvider
    {
        private Cache Kesz { get { return HttpContext.Current.Cache; } }

        public object Get(string key)
        {
            return Kesz[key];
        }

        public void Invalidate(string key)
        {
            Kesz.Remove(key);
        }

        public bool IsSet(string key)
        {
            return (Kesz[key] != null);
        }

        public void Set(string key, object data, int cacheTime)
        {
            var expirationTime = DateTime.Now + TimeSpan.FromMinutes(cacheTime);
            Kesz.Insert(key, data,null, expirationTime, Cache.NoSlidingExpiration);
        }
    }
}