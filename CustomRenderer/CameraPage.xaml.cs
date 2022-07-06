using CustomRenderer.Services;
using SkiaSharp.Views.Forms;
using SkiaSharp;
using System;
using Xamarin.Forms;

namespace CustomRenderer
{
	public partial class CameraPage : ContentPage
	{
        BarcodeQrData[] data = null;
        private int imageWidth;
        private int imageHeight;
        private float scaleFactor = 1.0f;
        private float postScaleWidthOffset;
        private float postScaleHeightOffset;
        public CameraPage ()
		{
			// A custom renderer is used to display the camera UI
			InitializeComponent ();
		}

		private void CameraPreview_ResultReady(object sender, ResultReadyEventArgs e)
		{
            if (e.Result != null)
            {
                data = (BarcodeQrData[])e.Result;
            }
            else
            {
                data = null;
            }

            imageWidth = e.PreviewWidth;
            imageHeight = e.PreviewHeight;

            canvasView.InvalidateSurface();
        }

        public float scale(float imagePixel)
        {
            return imagePixel * scaleFactor;
        }

        public static SKPoint rotateCW90(SKPoint point, int width)
        {
            SKPoint rotatedPoint = new SKPoint();
            rotatedPoint.X = width - point.Y;
            rotatedPoint.Y = point.X;
            return rotatedPoint;
        }

        void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            double width = canvasView.Width;
            double height = canvasView.Height;

            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.Clear();

            SKPaint skPaint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = SKColors.Blue,
                StrokeWidth = 10,
            };

            //canvas.DrawRect(new SKRect(100, 100, 200, 200), skPaint);

            ResultLabel.Text = "";
            if (data != null)
            {
                foreach (BarcodeQrData barcodeQrData in data)
                {
                    ResultLabel.Text += barcodeQrData.text + "\n";

                    for (int i = 0; i < 4; i++)
                    {
                        if (width < height)
                        {
                            barcodeQrData.points[i] = rotateCW90(barcodeQrData.points[i], imageHeight);
                        }
                        
                        barcodeQrData.points[i].Y = (float)(barcodeQrData.points[i].Y + (Application.Current.MainPage.Height - height));
                    }

                    //canvas.DrawText(barcodeQrData.text, new SKPoint(300, 300), skPaint);
                    canvas.DrawLine(barcodeQrData.points[0], barcodeQrData.points[1], skPaint);
                    canvas.DrawLine(barcodeQrData.points[1], barcodeQrData.points[2], skPaint);
                    canvas.DrawLine(barcodeQrData.points[2], barcodeQrData.points[3], skPaint);
                    canvas.DrawLine(barcodeQrData.points[3], barcodeQrData.points[0], skPaint);
                }
            }
            else
            {
                ResultLabel.Text = "No barcode QR code found";
            }

        }
    }
}

