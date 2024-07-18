// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace AdaptiveCards.Rendering.MAUI.Helpers
{
    public static class ControlExtensions
    {
        public static object GetContext(this View element)
        {
            return element?.BindingContext;
        }

        public static void SetContext(this View element, object value)
        {
            element.BindingContext = value;
        }

        public static void SetState(this CheckBox control, bool value)
        {
            control.IsChecked = value;
        }

        public static bool? GetState(this CheckBox control)
        {
            return control.IsChecked;
        }

        //public static void Add(this ListBox control, object element)
        //{
        //    control.Items.Add(element);
        //}

        public static void SetColor(this Label textBlock, AdaptiveTextColor color, bool isSubtle, AdaptiveRenderContext context)
        {
            FontColorConfig colorOption = context.GetForegroundColors(color);
            string colorCode = isSubtle ? colorOption.Subtle : colorOption.Default;
            textBlock.TextColor = context.GetColorBrush(colorCode).Color;
        }

        public static void SetColor(this Span inlineRun, AdaptiveTextColor color, bool isSubtle, AdaptiveRenderContext context)
        {
            FontColorConfig colorOption = context.GetForegroundColors(color);
            string colorCode = isSubtle ? colorOption.Subtle : colorOption.Default;
            inlineRun.TextColor = context.GetColorBrush(colorCode).Color;
        }

        public static void SetHighlightColor(this Span inlineRun, AdaptiveTextColor color, bool isSubtle, AdaptiveRenderContext context)
        {
            FontColorConfig colorOption = context.GetForegroundColors(color);
            string colorCode = isSubtle ? colorOption.HighlightColors.Subtle : colorOption.HighlightColors.Default;
            inlineRun.BackgroundColor = context.GetColorBrush(colorCode).Color;
        }

        public static void SetHorizontalAlignment(this Image image, AdaptiveHorizontalAlignment alignment)
        {
            switch (alignment)
            {
                case AdaptiveHorizontalAlignment.Left:
                    image.HorizontalOptions = LayoutOptions.Start;
                    break;
                case AdaptiveHorizontalAlignment.Center:
                    image.HorizontalOptions = LayoutOptions.Center;
                    break;
                case AdaptiveHorizontalAlignment.Right:
                    image.HorizontalOptions = LayoutOptions.End;
                    break;
                case AdaptiveHorizontalAlignment.Stretch:
                    image.HorizontalOptions = LayoutOptions.Fill;
                    break;
            }
        }

        public static void SetBackgroundColor(this Layout panel, string color, AdaptiveRenderContext context)
        {
            panel.Background = context.GetColorBrush(color);
        }

        public static void SetHeight(this View element, double height)
        {
            element.HeightRequest = height;
        }

        public static void SetBackgroundColor(this Button panel, string color, AdaptiveRenderContext context)
        {
            panel.Background = context.GetColorBrush(color);
        }

        public static void SetBorderColor(this Button view, string color, AdaptiveRenderContext context)
        {
            view.BorderColor = context.GetColorBrush(color).Color;
        }

        public static void SetBorderThickness(this Button view, double thickness)
        {
            view.BorderWidth = thickness;
        }

        public static void SetFontWeight(this Label textBlock, int weight)
        {
            //TODO FW textBlock.FontWeight = FontWeight.FromOpenTypeWeight(weight);
        }

        public static void SetPlaceholder(this InputView textBlock, string placeholder)
        {
            textBlock.Placeholder = placeholder;
        }
    }
}
