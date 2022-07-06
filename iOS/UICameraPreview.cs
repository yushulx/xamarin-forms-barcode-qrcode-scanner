using AVFoundation;
using CoreFoundation;
using CoreVideo;
using DBRiOS;
using Foundation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;

namespace CustomRenderer.iOS
{
    public class UICameraPreview : UIView
    {
        AVCaptureVideoPreviewLayer previewLayer;
        CameraOptions cameraOptions;
        private CaptureOutput captureOutput;
        DynamsoftBarcodeReader reader = new DynamsoftBarcodeReader();

        public event EventHandler<EventArgs> Tapped;
        CameraPreview cameraPreview;
        public AVCaptureSession CaptureSession { get; private set; }

        public bool IsPreviewing { get; set; }

        public UICameraPreview(CameraPreview preview)
        {
            cameraPreview = preview;
            captureOutput = new CaptureOutput(cameraPreview);
            cameraOptions = cameraPreview.Camera;
            IsPreviewing = false;
            Initialize();
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            if (previewLayer != null)
                previewLayer.Frame = Bounds;
        }

        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            base.TouchesBegan(touches, evt);
            OnTapped();
        }

        protected virtual void OnTapped()
        {
            var eventHandler = Tapped;
            if (eventHandler != null)
            {
                eventHandler(this, new EventArgs());
            }
        }

        void Initialize()
        {
            CaptureSession = new AVCaptureSession();
            previewLayer = new AVCaptureVideoPreviewLayer(CaptureSession)
            {
                Frame = Bounds,
                VideoGravity = AVLayerVideoGravity.ResizeAspectFill
            };

            var videoDevices = AVCaptureDevice.DevicesWithMediaType(AVMediaType.Video);
            var cameraPosition = (cameraOptions == CameraOptions.Front) ? AVCaptureDevicePosition.Front : AVCaptureDevicePosition.Back;
            var device = videoDevices.FirstOrDefault(d => d.Position == cameraPosition);

            if (device == null)
            {
                return;
            }

            NSError error;
            var input = new AVCaptureDeviceInput(device, out error);
            CaptureSession.AddInput(input);
            var videoDataOutput = new AVCaptureVideoDataOutput()
            {
                AlwaysDiscardsLateVideoFrames = true
            };
            if (CaptureSession.CanAddOutput(videoDataOutput))
            {
                CaptureSession.AddOutput(videoDataOutput);
                captureOutput.reader = reader;

                DispatchQueue queue = new DispatchQueue("camera");
                videoDataOutput.SetSampleBufferDelegateQueue(captureOutput, queue);
                videoDataOutput.WeakVideoSettings = new NSDictionary<NSString, NSObject>(CVPixelBuffer.PixelFormatTypeKey, NSNumber.FromInt32((int)CVPixelFormatType.CV32BGRA));
            }
            CaptureSession.CommitConfiguration();

            Layer.AddSublayer(previewLayer);
            CaptureSession.StartRunning();
            IsPreviewing = true;
        }

        public void Destroy()
        {
            CaptureSession?.Dispose();
        }
    }
}