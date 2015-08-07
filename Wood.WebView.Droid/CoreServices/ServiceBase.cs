using System;
using Android.OS;

namespace Wood.Core
{
    partial class ServiceBase
    {
        protected void RunOnMainThread(Action action)
        {
            Handler handler = new Handler(Looper.MainLooper);
            handler.Post(action);
            handler.Dispose();
            handler = null;
        }
    }
}