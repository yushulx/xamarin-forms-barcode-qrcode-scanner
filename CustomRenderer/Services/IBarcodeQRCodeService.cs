using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

//https://docs.microsoft.com/en-us/xamarin/xamarin-forms/app-fundamentals/dependency-service/
namespace CustomRenderer.Services
{
    public class BarcodeQrData
    {
        public string text;
        public string format;
        public SKPoint[] points;
    }

    public interface IBarcodeQRCodeService
    {
        Task<int> InitSDK(string license);
        Task<BarcodeQrData[]> DecodeFile(string filePath);
    }
}
