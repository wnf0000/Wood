using System;

using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Wood.Core;
using Android.Graphics;
using Android.Hardware;

namespace Wood.CoreService
{
    public class Camera : ServiceBase
    {
        public delegate void SuccessHandler(byte[] data);
        public override string ServiceName
        {
            get
            {
                return "Camera";
            }
        }
        public Camera()
        {

            //拍照
            AddMethod("capture", (core, args) =>
            {
                SuccessHandler act = null;
                act = (data) =>
                {
                    //Android.Widget.Toast.MakeText(Application.Context, data.ToString(), ToastLength.Long).Show();
                    CameraActivity.Success -= act;
                    var base64Str = Convert.ToBase64String(data);
                    core.InvokeCallback(args.CallbackName, base64Str);

                };
                CameraActivity.Success += act;

                (core.Context as Activity).StartActivity(typeof(CameraActivity));
            });
            //选取
            AddMethod("select", (core, args) => { });
        }

    }
    [Activity(Label = "Camera", Theme = "@android:style/Theme.Translucent.NoTitleBar.Fullscreen")]
    //[IntentFilter(new[] { "Camera_Activity" }, Categories = new[] { "android.intent.category.DEFAULT" },
    //Label = "照相机")]
    public class CameraActivity : Activity, ISurfaceHolderCallback, Android.Hardware.Camera.IShutterCallback, Android.Hardware.Camera.IPictureCallback, Android.Hardware.Camera.IAutoFocusCallback
    {

        public static event Camera.SuccessHandler Success;

        Android.Hardware.Camera camera;
        SurfaceView surfaceView;
        Button captureBtn, okBtn,redoBtn;
        byte[] pictureData;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Wood.WebView.Resource.Layout.Camera);
            surfaceView = (SurfaceView)FindViewById(Wood.WebView.Resource.Id.surfaceView);
            captureBtn = (Button)FindViewById(Wood.WebView.Resource.Id.capture);
            okBtn = (Button)FindViewById(Wood.WebView.Resource.Id.ok);
            redoBtn = (Button)FindViewById(Wood.WebView.Resource.Id.redo);
            ISurfaceHolder surfaceHolder = surfaceView.Holder;
            surfaceHolder.AddCallback(this);
            surfaceHolder.SetType(SurfaceType.PushBuffers);

            captureBtn.Click += delegate
            {
                camera.TakePicture(this, null, this);
            };
            okBtn.Click += delegate
            {
                if (Success != null) Success(pictureData);
                Finish();

            };
            redoBtn.Click += delegate {
                camera.StartPreview();
                redoBtn.Visibility = ViewStates.Gone;
                okBtn.Visibility = ViewStates.Gone;
                captureBtn.Visibility = ViewStates.Visible;
                
            };
        }

        public void SurfaceChanged(ISurfaceHolder holder, [GeneratedEnum] Format format, int width, int height)
        {

            //var parameters = camera.GetParameters();
            //parameters.SetPreviewSize(width, height);

            //IList<Android.Hardware.Camera.Size> vSizeList = parameters.SupportedPictureSizes;

            //for (int num = 0; num < vSizeList.Count; num++)

            //{

            //    var vSize = vSizeList[num];

            //}

            if (this.Resources.Configuration.Orientation != Android.Content.Res.Orientation.Landscape)

            {
                camera.SetDisplayOrientation(90);
            }

            else

            {
                camera.SetDisplayOrientation(0);
            }

            //camera.SetParameters(parameters);
            try
            {
                camera.SetPreviewDisplay(holder);

            }
            catch (Exception exception)
            {
                camera.Release();
                camera = null;
            }
            camera.StartPreview();
            //自动对焦
            camera.AutoFocus(this);
        }

        public void SurfaceCreated(ISurfaceHolder holder)
        {

            camera = Android.Hardware.Camera.Open(0);

        }

        public void SurfaceDestroyed(ISurfaceHolder holder)
        {
            //throw new NotImplementedException();
        }

        public void OnPictureTaken(byte[] data, Android.Hardware.Camera camera)
        {
            camera.StopPreview();
            pictureData = data;
            redoBtn.Visibility = ViewStates.Visible ;
            okBtn.Visibility = ViewStates.Visible;
            captureBtn.Visibility = ViewStates.Gone;
            //Android.Widget.Toast.MakeText(Application.Context, (data==null).ToString()+data.Length, ToastLength.Long).Show();
        }

        public void OnShutter()
        {
            //throw new NotImplementedException();
        }
        protected override void OnPause()
        {
            base.OnPause();
            camera.StopPreview();
            camera.Release();
        }

        public void OnAutoFocus(bool success, Android.Hardware.Camera camera)
        {
            //throw new NotImplementedException();
            if (success)
            {
                //
            }
        }

        //class PictureCallback :Java.Lang.Object, Android.Hardware.Camera.IPictureCallback
        //{
        //    string Name;
        //    public PictureCallback(string name)
        //    {
        //        Name = name;
        //    }
        //    public void OnPictureTaken(byte[] data, Android.Hardware.Camera camera)
        //    {
        //        Android.Widget.Toast.MakeText(Application.Context, Name+" "+(data == null).ToString(), ToastLength.Long).Show();
        //    }
        //}
    }
}