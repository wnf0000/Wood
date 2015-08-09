using UIKit;
using Wood.Core;

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
                BeginRunOnMainThread(() => {
                    UIAlertView alert = new UIAlertView();
                    alert.Message = args.GetPn(0, string.Empty);
                    alert.AddButton("OK");
                    alert.Show();
                });

            });
            
            this.AddMethod("showShort", (core, args) =>
            {
                BeginRunOnMainThread(() => {
                    UIAlertView alert = new UIAlertView();
                    alert.Message = args.GetPn(0, string.Empty);
                    alert.AddButton("OK");
                    alert.Show();
                });
            });
        }
    }
}