using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wood.Core
{
    public static class Extend
    {
        public static string ToJsonString(this object data)
        {
            var settings = new JsonSerializerSettings();
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            var json = JsonConvert.SerializeObject(data, Formatting.None, settings);
            return json;
        }
        public static string JsonString(object data)
        {
            if (data == null) return null;
            var settings = new JsonSerializerSettings();
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            var json = JsonConvert.SerializeObject(data, Formatting.None, settings);
            return json;
        }
        public static T DeserializeObject<T>(this string s) where T : class
        {
            if (string.IsNullOrEmpty(s)) return default(T);
            return JsonConvert.DeserializeObject<T>(s);
        }

        public static object DeserializeObject(this string s, Type type)
        {
            if (string.IsNullOrEmpty(s)) return null;
            return JsonConvert.DeserializeObject(s, type);
        }
    }
}
