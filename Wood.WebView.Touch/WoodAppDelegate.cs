using System;
using UIKit;
using Foundation;
using Wood.Core;

namespace Wood.WebView
{
	[Register("WoodAppDelegate")]
	public class WoodAppDelegate:UIApplicationDelegate
	{
		public override void DidEnterBackground (UIApplication application)
		{
			WoodCore.StopListion ();
		}
//		public override void OnActivated (UIApplication application)
//		{
//			
//		}
		public override void WillEnterForeground (UIApplication application)
		{
			WoodCore.StartListen ();
		}
	}
}

