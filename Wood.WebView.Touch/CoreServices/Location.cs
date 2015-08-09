using System;
using System.Collections.Generic;
using Wood.Core;
using System.Timers;

namespace Wood.CoreService
{
    class Location:ServiceBase
    {
        static readonly Dictionary<string, Timer> timers = new Dictionary<string, Timer>();
        public override string ServiceName
        {
            get { return "Location"; }
        }

        public Location()
        {
            //定位，这里只是模拟
            AddMethod("currentPos", (core,args) =>
            {
                core.InvokeCallback(args.CallbackName, new { lng = 100000, lat = 100 });
            });
            AddMethod("getPos", (core,args) =>
            {
                return new { lng = 100000, lat = 100 };
            });
            AddMethod("watch", (core,args) =>
            {
                var id = Guid.NewGuid().ToString("N");
                Timer timer = new Timer(1000);
                timer.Elapsed += delegate
                {
                    core.InvokeCallback(args.CallbackName, new { lng = DateTime.Now.Ticks, lat = 100 });
                };
                timer.Start();

                timers[id] = timer;
                return id;
            });
            AddMethod("unwatch", (core,args) =>
            {
                var id = args.GetPn(0,(string)null);
                if (!string.IsNullOrWhiteSpace(id) && timers.ContainsKey(id))
                {
                    var timer = timers[id];
                    timers.Remove(id);
                    if (timer != null)
                    {
                        timer.Stop();
                        timer.Dispose();
                        timer = null;
                    }
                }
            });
        }
    }
}