﻿using CustomRenderer.Services;
using SkiaSharp.Views.Forms;
using SkiaSharp;
using System;
using Xamarin.Forms;

namespace CustomRenderer
{
	public partial class CameraPage : ContentPage
	{
        BarcodeQrData[] data = null;

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
            canvasView.InvalidateSurface();
        }

        void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            double width = canvasView.Width;
            double height = canvasView.Height;
            double requestWidth = canvasView.WidthRequest;
            double requestHeight = canvasView.HeightRequest;
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

                    canvas.DrawText(barcodeQrData.text, new SKPoint(300, 300), skPaint);
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

