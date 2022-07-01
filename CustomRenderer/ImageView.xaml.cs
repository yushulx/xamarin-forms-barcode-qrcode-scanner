using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
//using Com.Dynamsoft.Dbr;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System.IO;
using System.Reflection;
using System.Net.Http;


namespace CustomRenderer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ImageView : ContentPage
    {
        string path;
        SKBitmap bitmap;
        //BarcodeReader barcodeReader;
        public ImageView(string imagepath)
        {
            InitializeComponent();
            ResultLabel.Text = imagepath;
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
            SKPaint skPaint = new SKPaint { Color = new SkiaSharp.SKColor(255, 0, 0, 255) };
            imageCanvas.DrawRect(100, 100, 300, 300, skPaint);

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