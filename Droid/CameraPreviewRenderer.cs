using System;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using CustomRenderer;
using CustomRenderer.Droid;
using Android.App;
using Android.Content;
using Android.Hardware;
using Android.Views;
using Android.Graphics;
using Android.Widget;
using Java.Interop;
using Xamarin.Forms.Platform.Android.FastRenderers;
using System.ComponentModel;

[assembly: ExportRenderer(typeof(CustomRenderer.CameraPreview), typeof(CameraPreviewRenderer))]
namespace CustomRenderer.Droid
{
    public class CameraPreviewRenderer : FrameLayout, IVisualElementRenderer, IViewRenderer, TextureView.ISurfaceTextureListener
    {
        global::Android.Hardware.Camera camera;
        global::Android.Widget.Button takePhotoButton;
        global::Android.Widget.Button toggleFlashButton;
        global::Android.Widget.Button switchCameraButton;
        global::Android.Views.View view;

        Activity activity;
        CameraFacing cameraType;
        TextureView textureView;
        SurfaceTexture surfaceTexture;
        CameraPreview element;
        VisualElementTracker visualElementTracker;
        VisualElementRenderer visualElementRenderer;
        FragmentManager fragmentManager;
        //CameraFragment cameraFragment;
        bool flashOn;
        int? defaultLabelFor;

        //FragmentManager FragmentManager => fragmentManager ??= Context.GetFragmentManager();

        public event EventHandler<VisualElementChangedEventArgs> ElementChanged;
        public event EventHandler<PropertyChangedEventArgs> ElementPropertyChanged;

        CameraPreview Element
        {
            get => element;
            set
            {
                if (element == value)
                {
                    return;
                }

                var oldElement = element;
                element = value;
                OnElementChanged(new ElementChangedEventArgs<CameraPreview>(oldElement, element));
            }
        }

        public CameraPreviewRenderer(Context context) : base(context)
        {
            visualElementRenderer = new VisualElementRenderer(this);
        }

        void OnElementChanged(ElementChangedEventArgs<CameraPreview> e)
        {
            //CameraFragment newFragment = null;

            if (e.OldElement != null)
            {
                e.OldElement.PropertyChanged -= OnElementPropertyChanged;
                //cameraFragment.Dispose();
            }
            if (e.NewElement != null)
            {
                this.EnsureId();

                e.NewElement.PropertyChanged += OnElementPropertyChanged;

                ElevationHelper.SetElevation(this, e.NewElement);
                //newFragment = new CameraFragment { Element = element };
            }

            //FragmentManager.BeginTransaction()
            //    .Replace(Id, cameraFragment = newFragment, "camera")
            //    .Commit();
            ElementChanged?.Invoke(this, new VisualElementChangedEventArgs(e.OldElement, e.NewElement));

            try
            {
                SetupUserInterface();
                SetupEventHandlers();
                AddView(view);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(@"			ERROR: ", ex.Message);
            }
        }

        async void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ElementPropertyChanged?.Invoke(this, e);

            switch (e.PropertyName)
            {
                case "Width":
                    //await cameraFragment.RetrieveCameraDevice();
                    break;
            }
        }

        //protected override void OnElementChanged(ElementChangedEventArgs<CameraPreview> e)
        //{
        //    base.OnElementChanged(e);

        //    if (e.OldElement != null || Element == null)
        //    {
        //        return;
        //    }

        //    try
        //    {
        //        SetupUserInterface();
        //        SetupEventHandlers();
        //        AddView(view);
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Diagnostics.Debug.WriteLine(@"			ERROR: ", ex.Message);
        //    }
        //}

        void SetupUserInterface()
        {
            activity = this.Context as Activity;
            view = activity.LayoutInflater.Inflate(Resource.Layout.CameraLayout, this, false);
            cameraType = CameraFacing.Back;

            textureView = view.FindViewById<TextureView>(Resource.Id.textureView);
            textureView.SurfaceTextureListener = this;
        }

        void SetupEventHandlers()
        {
            takePhotoButton = view.FindViewById<global::Android.Widget.Button>(Resource.Id.takePhotoButton);
            takePhotoButton.Click += TakePhotoButtonTapped;

            switchCameraButton = view.FindViewById<global::Android.Widget.Button>(Resource.Id.switchCameraButton);
            switchCameraButton.Click += SwitchCameraButtonTapped;

            toggleFlashButton = view.FindViewById<global::Android.Widget.Button>(Resource.Id.toggleFlashButton);
            toggleFlashButton.Click += ToggleFlashButtonTapped;
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            base.OnLayout(changed, l, t, r, b);

            var msw = MeasureSpec.MakeMeasureSpec(r - l, MeasureSpecMode.Exactly);
            var msh = MeasureSpec.MakeMeasureSpec(b - t, MeasureSpecMode.Exactly);

            view.Measure(msw, msh);
            view.Layout(0, 0, r - l, b - t);
        }

        public void OnSurfaceTextureUpdated(SurfaceTexture surface)
        {

        }

        public void OnSurfaceTextureAvailable(SurfaceTexture surface, int width, int height)
        {
            camera = global::Android.Hardware.Camera.Open((int)cameraType);
            textureView.LayoutParameters = new FrameLayout.LayoutParams(width, height);
            surfaceTexture = surface;

            camera.SetPreviewTexture(surface);
            PrepareAndStartCamera();
        }

        public bool OnSurfaceTextureDestroyed(SurfaceTexture surface)
        {
            camera.StopPreview();
            camera.Release();
            return true;
        }

        public void OnSurfaceTextureSizeChanged(SurfaceTexture surface, int width, int height)
        {
            PrepareAndStartCamera();
        }

        void PrepareAndStartCamera()
        {
            camera.StopPreview();

            var display = activity.WindowManager.DefaultDisplay;
            if (display.Rotation == SurfaceOrientation.Rotation0)
            {
                camera.SetDisplayOrientation(90);
            }

            if (display.Rotation == SurfaceOrientation.Rotation270)
            {
                camera.SetDisplayOrientation(180);
            }

            camera.StartPreview();
        }

        void ToggleFlashButtonTapped(object sender, EventArgs e)
        {
            flashOn = !flashOn;
            if (flashOn)
            {
                if (cameraType == CameraFacing.Back)
                {
                    toggleFlashButton.SetBackgroundResource(Resource.Drawable.FlashButton);
                    cameraType = CameraFacing.Back;

                    camera.StopPreview();
                    camera.Release();
                    camera = global::Android.Hardware.Camera.Open((int)cameraType);
                    var parameters = camera.GetParameters();
                    parameters.FlashMode = global::Android.Hardware.Camera.Parameters.FlashModeTorch;
                    camera.SetParameters(parameters);
                    camera.SetPreviewTexture(surfaceTexture);
                    PrepareAndStartCamera();
                }
            }
            else
            {
                toggleFlashButton.SetBackgroundResource(Resource.Drawable.NoFlashButton);
                camera.StopPreview();
                camera.Release();

                camera = global::Android.Hardware.Camera.Open((int)cameraType);
                var parameters = camera.GetParameters();
                parameters.FlashMode = global::Android.Hardware.Camera.Parameters.FlashModeOff;
                camera.SetParameters(parameters);
                camera.SetPreviewTexture(surfaceTexture);
                PrepareAndStartCamera();
            }
        }

        void SwitchCameraButtonTapped(object sender, EventArgs e)
        {
            if (cameraType == CameraFacing.Front)
            {
                cameraType = CameraFacing.Back;

                camera.StopPreview();
                camera.Release();
                camera = global::Android.Hardware.Camera.Open((int)cameraType);
                camera.SetPreviewTexture(surfaceTexture);
                PrepareAndStartCamera();
            }
            else
            {
                cameraType = CameraFacing.Front;

                camera.StopPreview();
                camera.Release();
                camera = global::Android.Hardware.Camera.Open((int)cameraType);
                camera.SetPreviewTexture(surfaceTexture);
                PrepareAndStartCamera();
            }
        }

        async void TakePhotoButtonTapped(object sender, EventArgs e)
        {
            camera.StopPreview();

            var image = textureView.Bitmap;

            try
            {
                var absolutePath = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDcim).AbsolutePath;
                var folderPath = absolutePath + "/Camera";
                var filePath = System.IO.Path.Combine(folderPath, string.Format("photo_{0}.jpg", Guid.NewGuid()));

                var fileStream = new FileStream(filePath, FileMode.Create);
                await image.CompressAsync(Bitmap.CompressFormat.Jpeg, 50, fileStream);
                fileStream.Close();
                image.Recycle();

                var intent = new Android.Content.Intent(Android.Content.Intent.ActionMediaScannerScanFile);
                var file = new Java.IO.File(filePath);
                var uri = Android.Net.Uri.FromFile(file);
                intent.SetData(uri);
                MainActivity.Instance.SendBroadcast(intent);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(@"				", ex.Message);
            }

            camera.StartPreview();
        }

        #region IViewRenderer

        void IViewRenderer.MeasureExactly() => MeasureExactly(this, Element, Context);

        static void MeasureExactly(Android.Views.View control, VisualElement element, Context context)
        {
            if (control == null || element == null)
            {
                return;
            }

            double width = element.Width;
            double height = element.Height;

            if (width <= 0 || height <= 0)
            {
                return;
            }

            int realWidth = (int)context.ToPixels(width);
            int realHeight = (int)context.ToPixels(height);

            int widthMeasureSpec = MeasureSpecFactory.MakeMeasureSpec(realWidth, MeasureSpecMode.Exactly);
            int heightMeasureSpec = MeasureSpecFactory.MakeMeasureSpec(realHeight, MeasureSpecMode.Exactly);

            control.Measure(widthMeasureSpec, heightMeasureSpec);
        }

        #endregion

        #region IVisualElementRenderer

        VisualElement IVisualElementRenderer.Element => Element;

        VisualElementTracker IVisualElementRenderer.Tracker => visualElementTracker;

        ViewGroup IVisualElementRenderer.ViewGroup => null;

        Android.Views.View IVisualElementRenderer.View => this;

        SizeRequest IVisualElementRenderer.GetDesiredSize(int widthConstraint, int heightConstraint)
        {
            Measure(widthConstraint, heightConstraint);
            SizeRequest result = new SizeRequest(new Size(MeasuredWidth, MeasuredHeight), new Size(Context.ToPixels(20), Context.ToPixels(20)));
            return result;
        }

        void IVisualElementRenderer.SetElement(VisualElement element)
        {
            if (!(element is CameraPreview camera))
            {
                throw new ArgumentException($"{nameof(element)} must be of type {nameof(CameraPreview)}");
            }

            if (visualElementTracker == null)
            {
                visualElementTracker = new VisualElementTracker(this);
            }
            Element = camera;
        }

        void IVisualElementRenderer.SetLabelFor(int? id)
        {
            if (defaultLabelFor == null)
            {
                defaultLabelFor = LabelFor;
            }
            LabelFor = (int)(id ?? defaultLabelFor);
        }

        void IVisualElementRenderer.UpdateLayout() => visualElementTracker?.UpdateLayout();

        #endregion

        static class MeasureSpecFactory
        {
            public static int GetSize(int measureSpec)
            {
                const int modeMask = 0x3 << 30;
                return measureSpec & ~modeMask;
            }

            public static int MakeMeasureSpec(int size, MeasureSpecMode mode) => size + (int)mode;
        }
    }
}

