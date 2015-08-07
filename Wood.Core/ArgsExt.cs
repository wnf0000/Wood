using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wood.Core
{
    public static class ArgsExt
    {
        public static T GetArgsOrDefault<T>(this JObject jObject, string arg, T def)
        {
            if (jObject == null) return def;
            JToken jt = null;
            if (jObject.TryGetValue(arg, out jt))
            {
                try
                {
                    return jt.ToObject<T>();
                }
                catch
                {
                    return def;
                }
                //var pp = jObject.GetValue(p);
                //if (pp != null) return pp.ToObject<T>();
                //return def;
            }
            else
            {
                return def;
            }

        }
    }
}
