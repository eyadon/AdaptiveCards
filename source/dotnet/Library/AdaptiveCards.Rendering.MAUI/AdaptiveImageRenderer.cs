//// Copyright (c) Microsoft Corporation. All rights reserved.
//// Licensed under the MIT License.

//using System.Drawing;
//using AdaptiveCards.Rendering.MAUI.Helpers;
//using Color = Microsoft.Maui.Graphics.Color;
//using Point = Microsoft.Maui.Graphics.Point;

//namespace AdaptiveCards.Rendering.MAUI
//{
//    public static class AdaptiveImageRenderer
//    {
//        public static View Render(AdaptiveImage image, AdaptiveRenderContext context)
//        {
//            View uiBorder = null;
//            var uiImage = new Image();

//            // Try to resolve the image URI
//            Uri finalUri = context.Config.ResolveFinalAbsoluteUri(image.Url);
//            if (finalUri == null)
//            {
//                return uiImage;
//            }

//            uiImage.SetSource(image, finalUri, context);

//            uiImage.SetHorizontalAlignment(image.HorizontalAlignment);

//            string style = $"Adaptive.{image.Type}";
//            if (image.Style == AdaptiveImageStyle.Person)
//            {
//                style += $".{image.Style}";

//                var mask = new RadialGradientBrush()
//                {
//                    GradientOrigin = new Point(0.5, 0.5),
//                    Center = new Point(0.5, 0.5),
//                    RadiusX = 0.5,
//                    RadiusY = 0.5,
//                    GradientStops = new GradientStopCollection()
//                };
//                mask.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#ffffffff"), 1.0));
//                mask.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#00ffffff"), 1.0));
//                uiImage.OpacityMask = mask;
//            }
//            uiImage.Style = context.GetStyle(style);

//            if (image.PixelHeight == 0 && image.PixelWidth == 0)
//            {
//                uiImage.SetImageProperties(image, context);
//            }

//            // If we have a background color, we'll create a border for the background and put the image on top
//            if (!string.IsNullOrEmpty(image.BackgroundColor))
//            {
//                Color color = (Color)ColorConverter.ConvertFromString(image.BackgroundColor);
//                if (color.Alpha != 0)
//                {
//                    uiBorder = new Border()
//                    {
//                        Background = new SolidColorBrush(color),
//                        Content = uiImage,
//                        WidthRequest = uiImage.Width,
//                        HeightRequest = uiImage.Height,
//                        HorizontalOptions = uiImage.HorizontalOptions,
//                        VerticalOptions = uiImage.VerticalOptions,
//                        OpacityMask = uiImage.OpacityMask
//                    };
//                }
//            }

//            return RendererUtil.ApplySelectAction(uiBorder ?? uiImage, image, context);
//        }

//    }
//}
