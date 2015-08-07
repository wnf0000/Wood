using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wood.Core
{
    public class ServiceArgs
    {
        public string Service { set; get; }
        public string Method { set; get; }

        public string CallbackName { set; get; }
        public bool RequireReturn { get; set; }
        public JObject Parms { set; get; }

        public T GetPn<T>(int n,T def)
        {
            return Parms.GetArgsOrDefault("p"+n, def);
        }
        
    }

}
