using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;


namespace Wood.WebView.AndroidSample
{
    [Activity(Label = "Wood.WebView.AndroidSample", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private Button goBtn;
        private EditText textUri;
        //private WoodWebView webView1;
        private WoodWebView webView2;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            //var webView = new CocoWebView(this);
            SetContentView(Resource.Layout.Main);
            //webView1 = FindViewById<WoodWebView>(Resource.Id.WoodWebView);
            webView2 = FindViewById<WoodWebView>(Resource.Id.WoodWebView2);
            textUri = FindViewById<EditText>(Resource.Id.textUri);
            goBtn = FindViewById<Button>(Resource.Id.goBtn);
            ////webView = new WoodWebView(this);
            ////SetContentView(webView);
            string url = "file:///android_asset/index.html";
            //webView.LoadUrl("javascript:{window.abcd=1;}");
            //webView.LoadUrl("javascript:{alert(window.abcd);}");
            //webView1.LoadUrl(url);
            webView2.LoadUrl(url);
            goBtn.Click += goBtn_Click;
        }

        private void goBtn_Click(object sender, EventArgs e)
        {
            string url = textUri.Text;
            if (string.IsNullOrWhiteSpace(url)) return;
            if (!url.StartsWith("http://"))
            {
                url = "http://" + url;
            }
            //webView1.LoadUrl(url);
            webView2.LoadUrl(url);
        }
    }
}

