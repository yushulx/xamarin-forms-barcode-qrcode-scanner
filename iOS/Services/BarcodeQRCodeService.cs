using System;

using Xamarin.Forms;
using BarcodeQrScanner.Services;
using DBRiOS;
using BarcodeQrScanner.iOS.Services;
using System.Threading.Tasks;
using Foundation;
using SkiaSharp;

[assembly: Dependency(typeof(BarcodeQRCodeService))]
namespace BarcodeQrScanner.iOS.Services
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
                NSError error;

                iPublicRuntimeSettings settings = reader.GetRuntimeSettings(out error);
                settings.ExpectedBarcodesCount = 512;
                reader.UpdateRuntimeSettings(settings, out error);
                
                iTextResult[] results = reader.DecodeFileWithName(filePath, "", out error);
                if (results != null && results.Length > 0)
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
