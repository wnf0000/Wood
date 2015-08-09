using CoreGraphics;
using Foundation;
using System;
using System.ComponentModel;
using UIKit;
using Wood.Core;

namespace Wood.WebView
{
    [Register("WoodWebView")]
    [DesignTimeVisible(true)]
    public class WoodWebView :UIWebView, IWebView
    {
        WoodCore _WoodCore;
        public WoodCore WoodCore
        {
            get { return _WoodCore; }
        }
        public WoodWebView() {
            Initialize();
        }
        public WoodWebView(IntPtr handle)
            : base(handle)
        {
            //Initialize();
        }
        public WoodWebView(CGRect frame) : base(frame)
        {
            Initialize();
        }
        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            Initialize();
        }
        void Initialize()
        {
            //Console.WriteLine("Initialize");
            _WoodCore = new WoodCore(this);
            this.LoadStarted += delegate { AttachCoreScripts(); };

        }

        public void ExecuteScript(string script)
        {
            BeginInvokeOnMainThread(()=>EvaluateJavascript(script));
        }

        public void AttachCoreScripts()
        {
            ExecuteScript(WoodCore.CoreScript);
        }
    }
}