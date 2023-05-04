using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using AndroidX.Core.Content;
using Com.Github.Alexzhirkevich.Customqrgenerator;
using Com.Github.Alexzhirkevich.Customqrgenerator.Style;
using Com.Github.Alexzhirkevich.Customqrgenerator.Vector;
using Com.Github.Alexzhirkevich.Customqrgenerator.Vector.Style;
using Picator.Game.Controls;
using Picator.Game.Droid.Renderers;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(QrImage), typeof(QrImageRenderer))]
namespace Picator.Game.Droid.Renderers
{
    public class QrImageRenderer : ImageRenderer
    {
        public QrImageRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Image> e)
        {
            base.OnElementChanged(e);
        }

        protected override async void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == VisualElement.WidthProperty.PropertyName)
                await UpdateQrData();
        }

        private async Task UpdateQrData()
        {
            var options = new QrVectorOptions.Builder()
                .SetOffset(new QrOffset(0, 0))
                .SetShapes(new QrVectorShapes(darkPixel: new QrVectorPixelShapeCircle(1F), QrVectorPixelShapeDefault.Instance, new QrVectorBallShapeCircle(1F), new QrVectorFrameShapeCircle(), true))
                .SetCodeShape(QrShapeDefault.Instance)
                .SetPadding(0)
                .SetColors(new QrVectorColors(dark: new QrVectorColorSolid(ColorConverters.FromHex("#3162e5").ToPlatformColor()), QrVectorColorUnspecified.Instance, QrVectorColorUnspecified.Instance, QrVectorColorUnspecified.Instance))
                .SetLogo(new QrVectorLogo(ContextCompat.GetDrawable(Context, Resource.Drawable.logo), size: 0.25F, padding: new QrVectorLogoPaddingNatural(0.2F), shape: QrVectorLogoShapeCircle.Instance, scale: BitmapScaleFitXY.Instance, QrVectorColorUnspecified.Instance))
                .SetBackground(new QrVectorBackground(drawable: null, BitmapScaleFitXY.Instance, QrVectorColorUnspecified.Instance))
                .SetErrorCorrectionLevel(QrErrorCorrectionLevel.Auto)
                .SetFourthEyeEnabled(true).Build();
            var data = (((QrImage)Element).Source as FileImageSource).File;
            var drawable = QrCodeDrawableKt.QrCodeDrawable(new QrDataText(data), options, null);
            var bitmap = drawableToBitmap(drawable);
            await ExportBitmapAsPNGAsync(bitmap);
            Control.SetImageDrawable(drawable);
        }

        private static Bitmap drawableToBitmap(Drawable drawable)
        {
            Bitmap bitmap = null;

            if (drawable is BitmapDrawable)
            {
                BitmapDrawable bitmapDrawable = (BitmapDrawable)drawable;
                if (bitmapDrawable.Bitmap != null)
                {
                    return bitmapDrawable.Bitmap;
                }
            }

            if (drawable.IntrinsicWidth <= 0 || drawable.IntrinsicHeight <= 0)
            {
                bitmap = Bitmap.CreateBitmap(512, 512, Bitmap.Config.Argb8888);
            }
            else
            {
                bitmap = Bitmap.CreateBitmap(drawable.IntrinsicWidth, drawable.IntrinsicHeight, Bitmap.Config.Argb8888);
            }

            Canvas canvas = new Canvas(bitmap);
            drawable.SetBounds(0, 0, canvas.Width, canvas.Height);
            drawable.Draw(canvas);
            return bitmap;
        }

        private async Task ExportBitmapAsPNGAsync(Bitmap bitmap)
        {
            if (DeviceInfo.Platform == DevicePlatform.Android && DeviceInfo.Version.Major < 13)
            {
                var granted = await RequestPermissions();
                if (!granted)
                    return;
            }
            var filePath = System.IO.Path.Combine(FileSystem.CacheDirectory, "qr.png");
            var stream = new FileStream(filePath, FileMode.OpenOrCreate);
            bitmap.Compress(Bitmap.CompressFormat.Png, 100, stream);
            stream.Close();
        }

        private async Task<bool> RequestPermissions()
        {
            var readStatus = await Permissions.CheckStatusAsync<Permissions.StorageRead>();
            var writeStatus = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();
            if (readStatus != PermissionStatus.Granted)
                readStatus = await Permissions.RequestAsync<Permissions.StorageRead>();
            if (writeStatus != PermissionStatus.Granted)
                writeStatus = await Permissions.RequestAsync<Permissions.StorageWrite>();
            if (readStatus != PermissionStatus.Granted || writeStatus != PermissionStatus.Granted)
                return false;
            return true;
        }
    }
}