using System;
using Wood.Core;
using UIKit;
using CoreAnimation;
using CoreGraphics;

namespace Wood.WebView
{
	public class Loading:ServiceBase
	{
		public override string ServiceName {
			get {
				return "Loading";
			}
		}
		public Loading ()
		{
			AddMethod ("show", (core, ArgsExt) => {
				BeginRunOnMainThread(()=>{
					UIApplication.SharedApplication.KeyWindow.AddSubview(new LoadingView());
				});

			});
			AddMethod ("dismiss", (core, ArgsExt) => {

			});
		}

		class LoadingView:UIView{
			public LoadingView(){
				Frame=new CoreGraphics.CGRect(new CoreGraphics.CGPoint(0,0),UIScreen.MainScreen.Bounds.Size);
				BackgroundColor=UIColor.FromRGBA(0,0,0,50);
				AutoresizingMask=UIViewAutoresizing.All;
				UIImage img=UIImage.FromBundle("1143454.gif");
				UIImageView imgView=new UIImageView(img);
				UIView.BeginAnimations(null);
				UIView.SetAnimationDuration(10);
				UIView.SetAnimationRepeatCount(10000000);

				imgView.Transform=CGAffineTransform.MakeRotation(360);
				UIView.CommitAnimations();
				AddSubview(imgView);

			}
		}
	}
}

