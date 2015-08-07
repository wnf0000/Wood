using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wood.Core
{
    public abstract partial class ServiceBase : IService, IServiceExecuter
    {
        #region const

        protected const string P0 = "p0";
        protected const string P1 = "p1";
        protected const string P2 = "p2";
        protected const string P3 = "p3";
        protected const string P4 = "p4";
        protected const string P5 = "p5";
        protected const string P6 = "p6";
        protected const string P7 = "p7";
        protected const string P8 = "p8";
        protected const string P9 = "p9";
        protected const string P10 = "p10";
        #endregion
        readonly Dictionary<string, object> _actions = new Dictionary<string, object>();
        public abstract string ServiceName { get; }
        //protected abstract void RunOnMainThread(Action action);
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
