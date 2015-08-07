using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Wood.Core
{
   public static class ServiceManager
    {
        private static readonly Dictionary<string, ServiceBase> Services = new Dictionary<string, ServiceBase>();
        static ServiceManager()
        {
            InitServices();
        }
        #region static methods
        private static void InitServices()
        {
            Type type = typeof(ServiceBase);
            Type[] types = Assembly.GetExecutingAssembly().GetTypes();
            //Assembly.Load("Wood.CoreService.dll").GetTypes()
            //    .Where(w=>w.FullName.StartsWith("Wood.CoreService")).ToArray();
            foreach (Type t in types)
            {
                if (t.IsClass)
                {
                    //try
                    //{
                    //    var instance = Activator.CreateInstance(t) as ServiceBase;
                    //    AddService(instance);
                    //}
                    //catch { }
                    if (t.IsSubclassOf(type))
                    {
                        var instance = Activator.CreateInstance(t) as ServiceBase;
                        AddService(instance);
                    }
                }
            }
        }

        public static void AddService(ServiceBase service)
        {
            Services.Add(service.ServiceName, service);
        }

        public static List<ServiceBase> GetServiceTypes()
        {
            return Services.Values.ToList();
        }

        public static ServiceBase GetService(string serviceName)
        {
            if (Services.ContainsKey(serviceName))
            {
                return Services[serviceName];
            }
            return null;
        }
        #endregion
    }
}
