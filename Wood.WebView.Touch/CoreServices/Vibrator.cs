using AudioToolbox;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wood.Core;

namespace Wood.CoreService
{
    class Vibrator : ServiceBase
    {
        SystemSound vibrator;
        public override string ServiceName
        {
            get { return "Vibrator"; }
        }
        public Vibrator()
        {

            vibrator = SystemSound.Vibrate;
            vibrator.CompletePlaybackIfAppDies = true;
            var cancel = false;
            AddMethod("vibrate", (core, args) =>
            {
                cancel = false;
                var pattern = args.GetPn<long[]>(0, null);
                if (pattern != null)
                {
                    var repeat = args.GetPn(1, -1);
                    var sleepArray = GetSleepArray(pattern);
                    Task.Run(() =>
                    {
                        for (int sleepIndex = 0; sleepIndex < sleepArray.Length; sleepIndex++)
                        {
                            if (cancel) break;
                            vibrator.PlaySystemSound();
                            if (sleepIndex == sleepArray.Length - 1)
                            {
                                if (repeat < 0) break;
                                sleepIndex = 0;
                            }
                        }
                    });
                }
                else
                {
                    //var duration = args.GetPn(0, 0);
                    vibrator.PlaySystemSound();
                }
            });
            AddMethod("cancel", (core, args) =>
            {
                cancel = true;
                vibrator.Close();
            });
        }

        int[] GetSleepArray(long[] pattern)
        {
            List<int> list = new List<int>();
            for (var i = 0; i < pattern.Length; i++)
            {
                if (i % 2 == 0)
                {
                    list.Add((int)pattern[i]);
                }
            }
            return list.ToArray();
        }
    }
}
