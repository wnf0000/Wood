
using Android.App;
using Android.Widget;
using Wood.Core;

namespace Wood.CoreService
{
    class Toast : ServiceBase
    {
        public override string ServiceName
        {
            get
            {
                return "Toast";
            }
        }
        public Toast()
        {
            this.AddMethod("showLong", (core, args) =>
            {
                RunOnMainThread(() => Android.Widget.Toast.MakeText(Application.Context, args.GetPn(0, string.Empty), ToastLength.Long).Show());
            });
            
            this.AddMethod("showShort", (core, args) =>
            {
                RunOnMainThread(() => Android.Widget.Toast.MakeText(Application.Context, args.GetPn(0, string.Empty), ToastLength.Short).Show());
            });
        }
    }
}