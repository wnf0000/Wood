using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wood.Core
{
    public abstract partial class ServiceBase : IService, IServiceExecuter
    {

        readonly Dictionary<string, object> _actions = new Dictionary<string, object>();
        public abstract string ServiceName { get; }

        protected void AddMethod(string methodName, Func<WoodCore, ServiceArgs, object> method)
        {
            _actions.Add(methodName, method);
        }

        protected void AddMethod(string methodName, Action<WoodCore, ServiceArgs> method)
        {
            _actions.Add(methodName, method);
        }

        public void Execute(WoodCore core, ServiceArgs args)
        {
            var methodName = args.Method;
            var callback = args.CallbackName;
            if (_actions.ContainsKey(methodName))
            {
                var method = _actions[methodName] as Action<WoodCore, ServiceArgs>;
                if (method == null)
                {
                    throw new Exception("service " + args.Service + " has no method named " + args.Method);
                }
                method(core, args);
                return;
            }
            throw new Exception("service " + args.Service + " has no method named " + args.Method);
        }

        public object ExecuteHasReturn(WoodCore core, ServiceArgs args)
        {
            var methodName = args.Method;
            var callback = args.CallbackName;
            if (_actions.ContainsKey(methodName))
            {
                var method = _actions[methodName] as Func<WoodCore, ServiceArgs, object>;
                if (method == null)
                {
                    throw new Exception("service " + args.Service + " has no method  named " + args.Method + " with return value");
                }
                return method(core, args);
            }
            throw new Exception("service " + args.Service + " has no method  named " + args.Method + " with return value");
        }


    }
}
