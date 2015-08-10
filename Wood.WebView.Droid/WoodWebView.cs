using System;
using Android.Content;
using Android.Runtime;
using Wood.Core;
using Android.Util;
using Android.OS;

namespace Wood.WebView
{
    public class WoodWebView :Android.Webkit.WebView, IWebView
    {
        WoodCore _WoodCore;
        public WoodCore WoodCore
        {
            get { return _WoodCore; }
            
        }

        protected WoodWebView(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
            Initialize();
        }

        public WoodWebView(Context context)
            : base(context)
        {
            Initialize();
        }

        public WoodWebView(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
            Initialize();
        }

        public WoodWebView(Context context, IAttributeSet attrs, int defStyle)
            : base(context, attrs, defStyle)
        {
            Initialize();
        }

        void Initialize()
        {
            //Console.WriteLine("Initialize");
            _WoodCore = new WoodCore(this);
            this.SetWebChromeClient(new WoodWebChromeClient());
            this.SetWebViewClient(new WoodWebViewClient());
            this.Settings.JavaScriptEnabled = true;
            this.Settings.JavaScriptCanOpenWindowsAutomatically = false;

        }

        public void ExecuteScript(string script)
        {
            Handler.Post(() => LoadUrl("javascript:{" + script + "}"));
            //LoadUrl("javascript:{" + script + "}");
        }

        public void AttachCoreScripts()
        {
            //Console.WriteLine(WoodCore.CoreScript);
            ExecuteScript(WoodCore.CoreScript);
        }

        public object GetContext()
        {
            return this.Context;
        }
    }
}