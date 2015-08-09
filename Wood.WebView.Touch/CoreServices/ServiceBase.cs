using System;
using UIKit;

namespace Wood.Core
{
    partial class ServiceBase
    {
        protected void RunOnMainThread(Action action)
        {
            UIApplication.SharedApplication.InvokeOnMainThread(action);
        }
        protected void BeginRunOnMainThread(Action action)
        {
            UIApplication.SharedApplication.BeginInvokeOnMainThread(action);
        }

    }
}