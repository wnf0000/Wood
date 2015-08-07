# Wood

这是个神奇的玩意 
Webview中javascript访问本地功能，
通过服务（插件）的形式，你可以把许许多多的本地功能无障碍的给提供给javascript调用
开发服务方法超级简单,继承ServiceBase类，
在构造函数里采用
AddMethod方法添加接口(方法)
<pre>
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
</pre>
