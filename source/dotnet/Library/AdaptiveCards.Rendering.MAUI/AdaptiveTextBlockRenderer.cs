// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Globalization;
using System.Xml;
using AdaptiveCards.Rendering.MAUI.Helpers;
using Microsoft.MarkedNet;
using GridLength = Microsoft.Maui.GridLength;
using TextAlignment = Microsoft.Maui.TextAlignment;

namespace AdaptiveCards.Rendering.MAUI
{
    public static class AdaptiveTextBlockRenderer
    {
        public static View Render(AdaptiveTextBlock textBlock, AdaptiveRenderContext context)
        {
            if (String.IsNullOrEmpty(textBlock.Text))
            {
                return null;
            }

            var uiTextBlock = CreateControl(textBlock, context);

            uiTextBlock.SetColor(textBlock.Color, textBlock.IsSubtle, context);

            if (textBlock.MaxWidth > 0)
            {
                uiTextBlock.MaximumWidthRequest = textBlock.MaxWidth;
            }

            if (textBlock.MaxLines > 0)
            {
                var uiGrid = new Grid();
                uiGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

                // create hidden textBlock with appropriate linebreaks that we can use to measure the ActualHeight
                // using same style, fontWeight settings as original textblock
                var measureBlock = new Label()
                {
                    Style = uiTextBlock.Style,
                    //TODO FW FontWeight = uiTextBlock.FontWeight,
                    FontSize = uiTextBlock.FontSize,
                    IsVisible = false,
                    LineBreakMode = LineBreakMode.NoWrap,
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.Start,
                    BindingContext = textBlock.MaxLines
                };

                measureBlock.FormattedText.Spans.Add(new Span() {Text = uiTextBlock.Text});

                // bind the real textBlock's Height => MeasureBlock.ActualHeight
                uiTextBlock.SetBinding(View.MaximumHeightRequestProperty, new Binding()
                {
                    Path = "Height",
                    Source = measureBlock,
                    Mode = BindingMode.OneWay,
                    Converter = new MultiplyConverter(textBlock.MaxLines)
                });

                // Add both to a grid so they go as a unit
                uiGrid.Children.Add(measureBlock);

                uiGrid.Children.Add(uiTextBlock);
                return uiGrid;

            }

            return uiTextBlock;
        }

        private static Label CreateControl(AdaptiveTextBlock textBlock, AdaptiveRenderContext context)
        {
            Marked marked = new Marked();
            marked.Options.Renderer = new AdaptiveXamlMarkdownRenderer();
            marked.Options.Mangle = false;
            marked.Options.Sanitize = true;

            string text = RendererUtilities.ApplyTextFunctions(textBlock.Text, context.Lang);

            text = marked.Parse(text);
            text = RendererUtilities.HandleHtmlSpaces(text);
            
            string xaml = $"<Label>{text}</Label>";

            var uiTextBlock = new Label().LoadFromXaml(xaml);
            uiTextBlock.Style = context.GetStyle($"Adaptive.{textBlock.Type}");

            uiTextBlock.LineBreakMode = LineBreakMode.NoWrap;

            //TODO FONT uiTextBlock.FontFamily = new FontFamily(RendererUtil.GetFontFamilyFromList(context.Config.GetFontFamily(textBlock.FontType)));
            //TODO FW uiTextBlock.FontWeight = FontWeight.FromOpenTypeWeight(context.Config.GetFontWeight(textBlock.FontType, textBlock.Weight));
            uiTextBlock.FontSize = context.Config.GetFontSize(textBlock.FontType, textBlock.Size);

            uiTextBlock.LineBreakMode = LineBreakMode.TailTruncation;

            if (textBlock.Italic)
            {
                uiTextBlock.FontAttributes = FontAttributes.Italic;
            }

            if (textBlock.Strikethrough)
            {
                uiTextBlock.TextDecorations = TextDecorations.Strikethrough;
            }

            if (textBlock.HorizontalAlignment == AdaptiveHorizontalAlignment.Right)
                uiTextBlock.HorizontalTextAlignment = TextAlignment.End;
            if (textBlock.HorizontalAlignment == AdaptiveHorizontalAlignment.Center)
                uiTextBlock.HorizontalTextAlignment = TextAlignment.Center;


            if (textBlock.Wrap)
                uiTextBlock.LineBreakMode = LineBreakMode.WordWrap;

            return uiTextBlock;
        }

        private class MultiplyConverter : IValueConverter
        {
            private int multiplier;

            public MultiplyConverter(int multiplier)
            {
                this.multiplier = multiplier;
            }

            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return (double)value * this.multiplier;
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return (double)value * this.multiplier;
            }
        }
    }
}
