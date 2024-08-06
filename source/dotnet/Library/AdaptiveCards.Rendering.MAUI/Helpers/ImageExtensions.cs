// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Globalization;
using ImageSource = Microsoft.Maui.Controls.ImageSource;
using Rect = Microsoft.Maui.Graphics.Rect;
using Stretch = Microsoft.Maui.Controls.Stretch;

namespace AdaptiveCards.Rendering.MAUI.Helpers
{
    public static class ImageExtensions
    {

        public class AdaptiveConverterParameters
        {
            public AdaptiveConverterParameters(Image image, AdaptiveImage adaptiveImage, AdaptiveRenderContext context)
            {
                Image = image;
                AdaptiveImage = adaptiveImage;
                AdaptiveContext = context;
            }
            public Image Image { get; set; }
            public AdaptiveImage AdaptiveImage { get; set; }
            public AdaptiveRenderContext AdaptiveContext { get; set; }
        }
        /// <summary>
        /// Renders the element to a bitmap
        /// </summary>
        //public static MemoryStream RenderToImage(this View element, int width)
        //{
        //    element.Measure(new Size(width, int.MaxValue));
        //    // Add 100 to the height to give it some buffer. This addressed some bugs with maxlines getting clipped
        //    element.Arrange(new Rect(new Size(width, element.DesiredSize.Height + 100)));
        //    element.UpdateLayout();

        //    var bitmapImage = new RenderTargetBitmap((int)width, (int)element.DesiredSize.Height, 96, 96,
        //        PixelFormats.Default);
        //    bitmapImage.Render(element);

        //    var encoder = new PngBitmapEncoder();
        //    var metadata = new BitmapMetadata("png");
        //    // TODO: Should we set the image metadata?
        //    //metadata.SetQuery("/tEXt/{str=Description}", JsonConvert.SerializeObject(OriginatingCard));
        //    var pngFrame = BitmapFrame.Create(bitmapImage, null, metadata, null);
        //    encoder.Frames.Add(pngFrame);

        //    var stream = new MemoryStream();
        //    encoder.Save(stream);
        //    stream.Seek(0, SeekOrigin.Begin);
        //    return stream;
        //}

        public static async void SetSource(this Image image, AdaptiveImage adaptiveImage, Uri url, AdaptiveRenderContext context)
        {
            if (url == null)
                return;

            image.Source = await context.ResolveImageSource(url);

            var parameters = new AdaptiveConverterParameters(image, adaptiveImage, context);
            var binding = new Binding
            {
                Source = Binding.SelfPath,
                Path = "Parent.Width",
                Mode = BindingMode.OneWay,
                //Converter = new StretchConverter(),
                //ConverterParameter = parameters
            };

            //image.SetBinding(Image.StretchProperty, binding);
        }

        public static async void SetSource(this Button button, AdaptiveAction adaptiveAction,
            AdaptiveRenderContext context)
        {
            Uri finalUri = context.Config.ResolveFinalAbsoluteUri(adaptiveAction.IconUrl);
            if (finalUri == null) return;
            button.ImageSource = await context.ResolveImageSource(finalUri);

        }

        //public class StretchConverter : IValueConverter
        //{
        //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        //    {
        //        var parentWidth = (double)value;
        //        var adaptiveParameters = (AdaptiveConverterParameters)parameter;
        //        var image = adaptiveParameters.Image;
        //        var adaptiveImage = adaptiveParameters.AdaptiveImage;
        //        var imageWidth = ((BitmapImage)image.Source)?.PixelWidth;
        //        var imageHeight = ((BitmapImage)image.Source)?.PixelHeight;

        //        if (adaptiveImage.PixelWidth != 0 || adaptiveImage.PixelHeight != 0)
        //        {
        //            if (adaptiveImage.PixelWidth == 0)
        //            {
        //                adaptiveImage.PixelWidth = (uint)((imageWidth / (float)imageHeight) * adaptiveImage.PixelHeight);
        //            }

        //            if (adaptiveImage.PixelHeight == 0)
        //            {
        //                adaptiveImage.PixelHeight = (uint)((imageHeight / (float)imageWidth) * adaptiveImage.PixelWidth);
        //            }

        //            image.WidthRequest = adaptiveImage.PixelWidth;
        //            image.HeightRequest = adaptiveImage.PixelHeight;

        //            return Stretch.Fill;
        //        }
        //        else
        //        {
        //            return Stretch.Uniform;
        //        }
        //    }

        //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        //public static BitmapImage GetBitmapFromBase64(Uri dataUri)
        //{
        //    var encodedData = dataUri.AbsoluteUri.Substring(dataUri.AbsoluteUri.LastIndexOf(',') + 1);

        //    var decodedDataUri = Convert.FromBase64String(encodedData);
        //    BitmapImage bitmap = new BitmapImage();
        //    bitmap.BeginInit();
        //    bitmap.StreamSource = new MemoryStream(decodedDataUri);
        //    bitmap.EndInit();

        //    return bitmap;
        //}

        public static async void SetBackgroundSource(this Grid grid, AdaptiveBackgroundImage adaptiveBackgroundImage, AdaptiveRenderContext context)
        {
            // Try to resolve the image URI
            Uri finalUri = context.Config.ResolveFinalAbsoluteUri(adaptiveBackgroundImage?.Url);
            if (finalUri == null)
            {
                return;
            }

            ImageSource bi = await context.ResolveImageSource(finalUri);

            if (bi != null)
            {
                // bi.Pixel{Width, Height}: dimensions of image
                // grid.Actual{Width, Height}: dimensions of grid containing background image
                switch (adaptiveBackgroundImage.FillMode)
                {
                    case AdaptiveImageFillMode.Repeat:
                        //grid.Background = new ImageBrush(bi)
                        //{
                        //    TileMode = Shader.TileMode.Tile,
                        //    Viewport = new Rect(0, 0, bi.PixelWidth, bi.PixelHeight),
                        //    ViewportUnits = BrushMappingMode.Absolute
                        //};
                        //break;
                    case AdaptiveImageFillMode.RepeatHorizontally:
                        //grid.Background = new ImageBrush(bi)
                        //{
                        //    TileMode = Shader.TileMode.FlipY,
                        //    Stretch = Stretch.Uniform,
                        //    AlignmentY = (AlignmentY)adaptiveBackgroundImage.VerticalAlignment,
                        //    Viewport = new Rect(0, 0, bi.PixelWidth, grid.ActualHeight + 1),
                        //    ViewportUnits = BrushMappingMode.Absolute
                        //};
                        //break;
                    case AdaptiveImageFillMode.RepeatVertically:
                        //grid.Background = new ImageBrush(bi)
                        //{
                        //    TileMode = Shader.TileMode.FlipX,
                        //    Stretch = Stretch.Uniform,
                        //    AlignmentX = (AlignmentX)adaptiveBackgroundImage.HorizontalAlignment,
                        //    Viewport = new Rect(0, 0, grid.ActualWidth + 1, bi.PixelWidth),
                        //    ViewportUnits = BrushMappingMode.Absolute
                        //};
                        //break;
                    case AdaptiveImageFillMode.Cover:
                    default:
                        //grid.Children.Add(new Image());// = new ImageBrush(bi)
                        //{
                        //    Stretch = Stretch.UniformToFill,
                        //    AlignmentY = (AlignmentY)adaptiveBackgroundImage.VerticalAlignment,
                        //    AlignmentX = (AlignmentX)adaptiveBackgroundImage.HorizontalAlignment
                        //};
                        throw new NotImplementedException("background image not supported yet");
                        break;
                }
            }
        }

        public static void SetImageProperties(this Image imageview, AdaptiveImage image, AdaptiveRenderContext context)
        {
            switch (image.Size)
            {
                case AdaptiveImageSize.Auto:
                    imageview.Aspect = Aspect.AspectFit;
                    break;
                case AdaptiveImageSize.Stretch:
                    imageview.Aspect = Aspect.AspectFit;
                    break;
                case AdaptiveImageSize.Small:
                    imageview.WidthRequest = context.Config.ImageSizes.Small;
                    imageview.HeightRequest = context.Config.ImageSizes.Small;
                    break;
                case AdaptiveImageSize.Medium:
                    imageview.WidthRequest = context.Config.ImageSizes.Medium;
                    imageview.HeightRequest = context.Config.ImageSizes.Medium;
                    break;
                case AdaptiveImageSize.Large:
                    imageview.WidthRequest = context.Config.ImageSizes.Large;
                    imageview.HeightRequest = context.Config.ImageSizes.Large;
                    break;
            }
        }
    }
}
