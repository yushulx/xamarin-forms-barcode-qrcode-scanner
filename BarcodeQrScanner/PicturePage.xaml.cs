using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using System.IO;
using System.Reflection;
using System.Net.Http;
using BarcodeQrScanner.Services;
using Xamarin.Forms.Shapes;


namespace BarcodeQrScanner
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PicturePage : ContentPage
    {
        string path;
        SKBitmap bitmap;
        IBarcodeQRCodeService _barcodeQRCodeService;
        public PicturePage(string imagepath, IBarcodeQRCodeService barcodeQRCodeService)
        {
            InitializeComponent();
            _barcodeQRCodeService = barcodeQRCodeService;
            path = imagepath;
            //ResultLabel.Text = imagepath;
            try
            {
                using (var stream = new SKFileStream(imagepath))
                {
                    bitmap = SKBitmap.Decode(stream);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        //https://docs.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/graphics/skiasharp/bitmaps/displaying
        async void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.Clear();

            var imageCanvas = new SKCanvas(bitmap);

            SKPaint skPaint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = SKColors.Blue,
                StrokeWidth = 10,
            };

            BarcodeQrData[] data = await _barcodeQRCodeService.DecodeFile(path);
            ResultLabel.Text = "";
            if (data != null)
            {
                foreach (BarcodeQrData barcodeQrData in data)
                {
                    ResultLabel.Text += barcodeQrData.text + "\n";
                    imageCanvas.DrawLine(barcodeQrData.points[0], barcodeQrData.points[1], skPaint);
                    imageCanvas.DrawLine(barcodeQrData.points[1], barcodeQrData.points[2], skPaint);
                    imageCanvas.DrawLine(barcodeQrData.points[2], barcodeQrData.points[3], skPaint);
                    imageCanvas.DrawLine(barcodeQrData.points[3], barcodeQrData.points[0], skPaint);
                }
            }
            else
            {
                ResultLabel.Text = "No barcode QR code found";
            }

            float scale = Math.Min((float)info.Width / bitmap.Width,
                               (float)info.Height / bitmap.Height);
            float x = (info.Width - scale * bitmap.Width) / 2;
            float y = (info.Height - scale * bitmap.Height) / 2;
            SKRect destRect = new SKRect(x, y, x + scale * bitmap.Width,
                                               y + scale * bitmap.Height);

            canvas.DrawBitmap(bitmap, destRect);
        }
    }
}