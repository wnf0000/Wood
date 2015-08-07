using Android.Webkit;
using System;

namespace Wood.WebView
{
    class WoodWebViewClient: WebViewClient
    {
        public override void OnPageFinished(Android.Webkit.WebView view, string url)
        {
            //Console.WriteLine("OnPageFinished");
            base.OnPageFinished(view, url);
            var WoodWebView = (view as WoodWebView);
            WoodWebView.AttachCoreScripts();
        }
    }
}