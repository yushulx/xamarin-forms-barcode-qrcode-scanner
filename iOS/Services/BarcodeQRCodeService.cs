using System;

using Xamarin.Forms;
using CustomRenderer.Services;
using DBRiOS;
using CustomRenderer.iOS.Services;
using System.Threading.Tasks;
using Foundation;
using SkiaSharp;

[assembly: Dependency(typeof(BarcodeQRCodeService))]
namespace CustomRenderer.iOS.Services
{
    public class DBRLicenseVerificationListener : NSObject, IDBRLicenseVerificationListener
    {
        public void DBRLicenseVerificationCallback(bool isSuccess, NSError error)
        {
            if (error != null)
            {
                System.Console.WriteLine(error.UserInfo);
            }
        }
    }

    public class BarcodeQRCodeService: IBarcodeQRCodeService
    {
        DynamsoftBarcodeReader reader;

        

        Task<int> IBarcodeQRCodeService.InitSDK(string license)
        {
            DynamsoftBarcodeReader.InitLicense(license, new DBRLicenseVerificationListener());
            reader = new DynamsoftBarcodeReader();
            TaskCompletionSource<int> taskCompletionSource = new TaskCompletionSource<int>();
            taskCompletionSource.SetResult(0);
            return taskCompletionSource.Task;
        }

        Task<BarcodeQrData[]> IBarcodeQRCodeService.DecodeFile(string filePath)
        {
            BarcodeQrData[] output = null;
            try
            {
                NSError nSError;
                iTextResult[] results = reader.DecodeFileWithName(filePath, "", out nSError);
                if (results != null)
                {
                    output = new BarcodeQrData[results.Length];
                    int index = 0;
                    foreach (iTextResult result in results)
                    {
                        BarcodeQrData data = new BarcodeQrData();
                        data.text = result.BarcodeText;
                        data.format = result.BarcodeFormatString;
                        iLocalizationResult localizationResult = result.LocalizationResult;
                        data.points = new SKPoint[localizationResult.ResultPoints.Length];
                        int pointsIndex = 0;
                        foreach (NSObject point in localizationResult.ResultPoints)
                        {
                            SKPoint p = new SKPoint();
                            p.X = (float)((NSValue)point).CGPointValue.X;
                            p.Y = (float)((NSValue)point).CGPointValue.Y;
                            data.points[pointsIndex++] = p;
                        }
                        output[index++] = data;
                    }
                }
            }
            catch (Exception e)
            {
            }

            TaskCompletionSource<BarcodeQrData[]> taskCompletionSource = new TaskCompletionSource<BarcodeQrData[]>();
            taskCompletionSource.SetResult(output);
            return taskCompletionSource.Task;
        }

    }
}
