using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Wood.Core;

namespace Wood.CoreService
{
    class Loading : ServiceBase
    {
        public override string ServiceName
        {
            get
            {
                return "Loading";
            }
        }
       public Loading()
        {
            AddMethod("show", (core, args) => { });
            AddMethod("hide", (core, args) => { });
        }
    }
}