# Xamarin Forms Barcode QR Code Scanner

This sample demonstrates how to scan barcode and QR code from image file and live video stream using [Xamarin.Forms Custom Renderers](https://docs.microsoft.com/en-us/xamarin/xamarin-forms/app-fundamentals/custom-renderer/) and [Dynamsoft Barcode Reader SDK](https://www.dynamsoft.com/barcode-reader/overview/).

## Usage
1. Import the project to `Visual Studio`.
2. Apply for a [30-day free trial license](https://www.dynamsoft.com/customer/license/trialLicense?product=dbr) and update the following code in [MainPage.xaml.cs](https://github.com/yushulx/xamarin-forms-barcode-qrcode-scanner/blob/main/CustomRenderer/MainPage.xaml.cs).

    ```csharp
    _barcodeQRCodeService.InitSDK("DLS2eyJoYW5kc2hha2VDb2RlIjoiMjAwMDAxLTE2NDk4Mjk3OTI2MzUiLCJvcmdhbml6YXRpb25JRCI6IjIwMDAwMSIsInNlc3Npb25QYXNzd29yZCI6IndTcGR6Vm05WDJrcEQ5YUoifQ==");
    ```
3. Set `android` or `iOS` as the start project.
4. Connect your devices and run the project. 

