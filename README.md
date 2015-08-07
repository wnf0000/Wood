# Wood

这是个神奇的玩意 
Webview中javascript访问本地功能，
通过服务（插件）的形式，你可以把许许多多的本地功能无障碍的给提供给javascript调用
开发服务方法超级简单,继承ServiceBase类，
在构造函数里采用
AddMethod方法添加接口(方法)即可。

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
服务需要在javascript注册下才能通过js调用，注册方式：修改WoodCore.cs中的CoreScript属性，把返回值中底下的那部分代码修改下即可,注意js书写要规整，字符串建议用单引号，不要用双引号，表达式要用分号结束，不要空着。
<pre>
(function () {
    Wood.RegisterService('Toast')
    .RegisterMethod('showLong', ['text'])
    .RegisterMethod('showShort', ['text']);

    Wood.RegisterService('Vibrator')
        .RegisterMethod('vibrate', ['milliseconds'])
        .RegisterMethod('vibrate', ['pattern', 'repeat'])
        .RegisterMethod('cancel', []);

    Wood.RegisterService('Location')
        .RegisterMethod('currentPos', [], false)
        .RegisterMethod('getPos', [], true)
        .RegisterMethod('watch', [], true)
        .RegisterMethod('unwatch', []);

})();
</rep>
