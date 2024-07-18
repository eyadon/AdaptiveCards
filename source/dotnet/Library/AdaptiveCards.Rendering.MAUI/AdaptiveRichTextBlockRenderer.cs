//// Copyright (c) Microsoft Corporation. All rights reserved.
//// Licensed under the MIT License.

//using System.Globalization;
//using AdaptiveCards.Rendering.MAUI.Helpers;

//namespace AdaptiveCards.Rendering.MAUI
//{
//    public static class AdaptiveRichTextBlockRenderer
//    {
//        public static View Render(AdaptiveRichTextBlock richTB, AdaptiveRenderContext context)
//        {
//            var uiRichTB = CreateControl(richTB, context);

//            foreach (var inlineElement in richTB.Inlines)
//            {
//                AdaptiveTextRun textRun = inlineElement as AdaptiveTextRun;
//                AddInlineTextRun(uiRichTB, textRun, context);
//            }

//            return uiRichTB;
//        }

//        private static void AddInlineTextRun(Label uiRichTB, AdaptiveTextRun textRun, AdaptiveRenderContext context)
//        {
//            Span textRunSpan;
//            if (textRun.SelectAction != null && context.Config.SupportsInteractivity)
//            {
//                Hyperlink selectActionLink = new Hyperlink();
//                selectActionLink.Click += (sender, e) =>
//                {
//                    context.InvokeAction(uiRichTB, new AdaptiveActionEventArgs(textRun.SelectAction));
//                    e.Handled = true;
//                };

//                textRunSpan = selectActionLink as Span;

//                if (textRun.SelectAction.Title != null)
//                {
//                    AutomationProperties.SetName(textRunSpan, textRun.SelectAction.Title);
//                }
//            }
//            else
//            {
//                textRunSpan = new Span();
//            }

//            // Handle Date/Time parsing
//            string text = RendererUtilities.ApplyTextFunctions(textRun.Text, context.Lang);

//            textRunSpan.Inlines.Add(text);

//            textRunSpan.Style = context.GetStyle($"Adaptive.{textRun.Type}");

//            textRunSpan.FontFamily = new FontFamily(RendererUtil.GetFontFamilyFromList(context.Config.GetFontFamily(textRun.FontType)));

//            textRunSpan.FontWeight = FontWeight.FromOpenTypeWeight(context.Config.GetFontWeight(textRun.FontType, textRun.Weight));
            
//            textRunSpan.FontSize = context.Config.GetFontSize(textRun.FontType, textRun.Size);

//            if (textRun.Italic)
//            {
//                textRunSpan.FontStyle = FontStyles.Italic;
//            }

//            if (textRun.Strikethrough)
//            {
//                textRunSpan.TextDecorations.Add(TextDecorations.Strikethrough);
//            }
            
//            if (textRun.Underline)
//            {
//                textRunSpan.TextDecorations.Add(TextDecorations.Underline);
//            }

//            if (textRun.Highlight)
//            {
//                textRunSpan.SetHighlightColor(textRun.Color, textRun.IsSubtle, context);
//            }

//            textRunSpan.SetColor(textRun.Color, textRun.IsSubtle, context);

//            uiRichTB.Inlines.Add(textRunSpan);
//        }

//        private static Label CreateControl(AdaptiveRichTextBlock richTB, AdaptiveRenderContext context)
//        {
//            Label uiTextBlock = new Label();
//            uiTextBlock.Style = context.GetStyle($"Adaptive.{richTB.Type}");
//            uiTextBlock.LineBreakMode = LineBreakMode.CharacterWrap;
            
//            if (richTB.HorizontalAlignment == AdaptiveHorizontalAlignment.Right)
//                uiTextBlock.HorizontalTextAlignment = TextAlignment.End;
//            if (richTB.HorizontalAlignment == AdaptiveHorizontalAlignment.Center)
//                uiTextBlock.HorizontalTextAlignment = TextAlignment.Center;

//            return uiTextBlock;
//        }

//        private static FontWeight SelectMaxTextWeight(FontWeight first, FontWeight second)
//        {
//            return (first > second) ? first : second;
//        }

//        private class MultiplyConverter : IValueConverter
//        {
//            private int multiplier;

//            public MultiplyConverter(int multiplier)
//            {
//                this.multiplier = multiplier;
//            }

//            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
//            {
//                return (double)value * this.multiplier;
//            }

//            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
//            {
//                return (double)value * this.multiplier;
//            }
//        }
//    }
//}
