using System;

using Android.App;
using Android.Content;
using Wood.Core;

namespace Wood.CoreService
{
   class Vibrator : ServiceBase
    {
        private Lazy<Android.OS.Vibrator> vibrator;
        public override string ServiceName
        {
            get { return "Vibrator"; }
        }
        public Vibrator()
        {
            vibrator = new Lazy<Android.OS.Vibrator>(() =>
            {
                return (Android.OS.Vibrator)Application.Context.GetSystemService(Context.VibratorService);
            });

            AddMethod("vibrate", (core,args) =>
            {

                var pattern = args.GetPn<long[]>(0,null);
                if (pattern != null)
                {
                    var repeat = args.GetPn(1, -1);
                    vibrator.Value.Vibrate(pattern, repeat);
                }
                else
                {
                    var duration = args.GetPn(0, 0);
                    if (duration > 0)
                    {
                        vibrator.Value.Vibrate((long)duration);
                    }
                }
            });
            AddMethod("cancel", (core,args) =>
            {
                vibrator.Value.Cancel();
            });
        }
    }
}