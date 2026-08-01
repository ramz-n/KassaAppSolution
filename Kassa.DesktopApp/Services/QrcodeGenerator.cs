using QRCoder;
using System.IO;
using System.Windows.Media.Imaging;

namespace Kassa.DesktopApp.Services
{
    public static class QrCodeImageBuilder
    {
        public static BitmapImage GeneratePng(string payload, int pixelsPerModule = 10)
        {
            using var generator = new QRCodeGenerator();
            using var qrData = generator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
            var pngQrCode = new PngByteQRCode(qrData);
            var pngBytes = pngQrCode.GetGraphic(pixelsPerModule);

            var image = new BitmapImage();
            using var stream = new MemoryStream(pngBytes);
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad; 
            image.StreamSource = stream;
            image.EndInit();
            image.Freeze();

            return image;
        }
    }
}
