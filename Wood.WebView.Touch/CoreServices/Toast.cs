using UIKit;
using Wood.Core;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Drawing;
using Foundation;
using CoreGraphics;

namespace Wood.CoreService
{
    class Toast : ServiceBase
    {
        public override string ServiceName
        {
            get
            {
                return "Toast";
            }
        }
        public Toast()
        {
            this.AddMethod("showLong", (core, args) =>
            {
//                BeginRunOnMainThread(() => {
//                    UIAlertView alert = new UIAlertView();
//                    alert.Message = args.GetPn(0, string.Empty);
//                    alert.AddButton("OK");
//                    alert.Show();
//                });
					AppMsg.MakeText(args.GetPn(0, string.Empty),5000).Show();
            });
            
            this.AddMethod("showShort", (core, args) =>
            {
//                BeginRunOnMainThread(() => {
//                    UIAlertView alert = new UIAlertView();
//                    alert.Message = args.GetPn(0, string.Empty);
//                    alert.AddButton("OK");
//                    alert.Show();
//                });
					AppMsg.MakeText(args.GetPn(0, string.Empty),3000).Show();
            });
        }
    }
	class MsgManager:NSObject
	{
		
		private static MsgManager mInstance;
		private readonly ManualResetEvent allDone = new ManualResetEvent(false);


		private readonly Queue<AppMsg> msgQueue;

		private bool IsRunning;
		private bool needGoon;

		private MsgManager()
		{
			msgQueue = new Queue<AppMsg>();
		}

		public static MsgManager Instance
		{
			get
			{
				if (mInstance == null)
				{
					mInstance = new MsgManager();
				}
				return mInstance;
			}
		}

		private void Start()
		{
			IsRunning = true;
			Task.Run(() =>
				{
					while (needGoon)
					{
						//allDone.Reset();
						allDone.WaitOne();
						//allDone.Set();
						DisplayMsg();
						allDone.Reset();
					}
					IsRunning = false;
				});
		}

		public void Add(AppMsg appMsg)
		{
			msgQueue.Enqueue (appMsg);
			needGoon = true;
			if (!IsRunning) {
				
				Start ();
				allDone.Set();

			}
		}

		public void ClearMsg(AppMsg appMsg)
		{
			if (msgQueue.Contains(appMsg))
			{
				appMsg.State = MsgState.Removed;
			}
		}


		public void ClearAllMsg()
		{
			if (msgQueue != null)
			{
				msgQueue.Clear();
			}
		}

		private void DisplayMsg()
		{
			if (msgQueue.Count == 0)
			{
				needGoon = false;
				allDone.Set();
				return;
			}

			AppMsg appMsg = msgQueue.Dequeue();

			AddMsgToView(appMsg);
			//allDone.Set();
		}

		private void AddMsgToView(AppMsg appMsg)
		{
			UIApplication.SharedApplication.InvokeOnMainThread(() =>
				{
					var size = UIStringDrawing.StringSize (new NSString (appMsg.Text), UIFont.BoldSystemFontOfSize (14f), UIScreen.MainScreen.Bounds.Width - 80, UILineBreakMode.WordWrap);
					int paddingh=15;
					int paddingv=8;
					var inner = new UILabel(
						new CoreGraphics.CGRect(
							(UIScreen.MainScreen.Bounds.Width-size.Width)/2,
							(UIScreen.MainScreen.Bounds.Bottom - size.Height) / 2,
							size.Width,
							size.Height
						)
					)
					{
						Text = appMsg.Text,
						Lines=0,
						Font = UIFont.BoldSystemFontOfSize(14f),
						TextColor = UIColor.White,
						BackgroundColor=UIColor.FromWhiteAlpha(0,0),
						TextAlignment = UITextAlignment.Center,
						LineBreakMode=UILineBreakMode.WordWrap

					};

					inner.SizeToFit();
					var rect=inner.Frame;
					var zeroSizeRect=rect;
					zeroSizeRect.X+=(rect.Width-0)/2;
					zeroSizeRect.Y+=rect.Height/2;
					zeroSizeRect.Size=new CGSize(0,0);
					var outRect=new CGRect(rect.X-paddingh,rect.Y-paddingv,rect.Width+paddingh*2,rect.Height+paddingv*2);
					var layout=new UIView(zeroSizeRect){
						BackgroundColor = UIColor.FromRGBA(0,0,0,0)
					};
					layout.Layer.CornerRadius=8;
					layout.Layer.MasksToBounds=true;
					layout.Layer.BorderWidth=0;
					inner.Frame=new CGRect(paddingh,paddingv,rect.Width,rect.Height);
					layout.AddSubview(inner);
					UIApplication.SharedApplication.KeyWindow.AddSubview(layout);
					UIView.AnimateAsync(0.5,()=>{
						layout.BackgroundColor=UIColor.FromRGBA(0,0,0,200);
						layout.Frame=outRect;
					});

					appMsg.State = MsgState.IsShowing;
					Task.Delay(appMsg.Duration-1000).ContinueWith(r => UIApplication.SharedApplication.InvokeOnMainThread(() =>
						{

							UIView.AnimateAsync(.5,()=>{
								layout.BackgroundColor=layout.BackgroundColor.ColorWithAlpha(0);
							}).ContinueWith(t=>{
								InvokeOnMainThread (()=>{
									layout.Hidden=true;
									layout.RemoveFromSuperview();
								
								});

							});
							appMsg.State = MsgState.Display;
							if (msgQueue.Count == 0)
							{
								needGoon = false;
							}
							allDone.Set();
						}));
				});
		}


	}
	enum MsgState{
		Added = 1,
		Display = 2,
		Removed = 3,
		IsShowing = 4
	}
	class AppMsg{
		AppMsg(string text,int duration){
			this.Text=text;
			this.Duration=duration;
		}
		public string Text{ set; get; }
		public int Duration{ set; get; }
		public MsgState State{ set; get; }
		public AppMsg Show()
		{
			MsgManager manager = MsgManager.Instance;
			manager.Add(this);
			return this;
		}
		public static AppMsg MakeText( string text, int duration)
		{
			var appMsg = new AppMsg(text,duration);
			return appMsg;
		}
	}

}