﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiddenWatermark
{
    internal static class ScaledWatermarkCache
    {
        private const int CacheSize = 10;

        private static ConcurrentDictionary<string, int> _count = new ConcurrentDictionary<string, int>();
        private static ConcurrentDictionary<string, byte[]> _cache = new ConcurrentDictionary<string, byte[]>();

        public static byte[] TryGetScaledWatermark(int width, int height)
        {
            var key = ToKey(width, height);
            _count[key] = _count.ContainsKey(key) ? _count[key] + 1 : 1;
            if (_cache.ContainsKey(key))
            {
                return _cache[key];
            }
            return null;
        }

        public static void AddScaledWatermark(int width, int height, byte[] watermarkBytes)
        {
            var key = ToKey(width, height);
            _cache[key] = watermarkBytes;
            TrimCache();
        }

        private static void TrimCache()
        {
            if (_cache.Count > CacheSize)
            {
                var min = _count.Values.Min();
                foreach (var pair in _count)
                {
                    if (pair.Value == min)
                    {
                        byte[] val;
                        _cache.TryRemove(pair.Key, out val);
                        break;
                    }
                }
            }
        }

        private static string ToKey(int width, int height)
        {
            return String.Format("{0}x{1}", width, height);
        }
    }
}