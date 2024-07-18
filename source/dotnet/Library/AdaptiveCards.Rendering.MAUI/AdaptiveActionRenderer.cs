// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace AdaptiveCards.Rendering.MAUI
{
    public static class AdaptiveActionRenderer
    {
        public static View Render(AdaptiveAction action, AdaptiveRenderContext context)
        {
            if (context.Config.SupportsInteractivity && context.ActionHandlers.IsSupported(action.GetType()))
            {
                var uiButton = CreateActionButton(action, context);
                var tap = new TapGestureRecognizer();
                tap.Command = new Command(() =>
                    context.InvokeAction(uiButton, new AdaptiveActionEventArgs(action)));

                uiButton.GestureRecognizers.Add(tap);
                //uiButton.Clicked += (sender, e) =>
                //{
                //    context.InvokeAction(uiButton, new AdaptiveActionEventArgs(action));

                //    // Prevent nested events from triggering
                //    //e.Handled = true;
                //};
                return uiButton;
            }
            return null;
        }

        public static Frame CreateActionButton(AdaptiveAction action, AdaptiveRenderContext context)
        {
            var uiButton = new Frame
            {
                Style = context.GetStyle($"Adaptive.{action.Type}"),
            };

            if (!String.IsNullOrWhiteSpace(action.Style))
            {
                Style style = context.GetStyle($"Adaptive.Action.{action.Style}");

                if (style == null && String.Equals(action.Style, "positive", StringComparison.OrdinalIgnoreCase))
                {
                    style = context.GetStyle("PositiveActionDefaultStyle");
                }
                else if (style == null && String.Equals(action.Style, "destructive", StringComparison.OrdinalIgnoreCase))
                {
                    style = context.GetStyle("DestructiveActionDefaultStyle");
                }

                uiButton.Style = style;
            }

            var contentStackPanel = new StackLayout();

            if (!context.IsRenderingSelectAction)
            {
                // Only apply padding for normal card actions
                uiButton.Padding = new Thickness(6, 4, 6, 4);
            }
            else
            {
                // Remove any extra spacing for selectAction
                uiButton.Padding = new Thickness(0, 0, 0, 0);
                contentStackPanel.Margin = new Thickness(0, 0, 0, 0);
            }
            uiButton.Content = contentStackPanel;
            View uiIcon = null;

            var uiTitle = new Label
            {
                Text = action.Title,
                FontSize = context.Config.GetFontSize(AdaptiveFontType.Default, AdaptiveTextSize.Default),
                VerticalOptions = LayoutOptions.Center,
                Style = context.GetStyle($"Adaptive.Action.Title")
            };

            if (action.IconUrl != null)
            {
                var actionsConfig = context.Config.Actions;

                var image = new AdaptiveImage(action.IconUrl)
                {
                    HorizontalAlignment = AdaptiveHorizontalAlignment.Center
                };
                //TODO IMG uiIcon = AdaptiveImageRenderer.Render(image, context);

                if (actionsConfig.IconPlacement == IconPlacement.AboveTitle)
                {
                    contentStackPanel.Orientation = StackOrientation.Vertical;
                    uiIcon.HeightRequest = (double)actionsConfig.IconSize;
                }
                else
                {
                    contentStackPanel.Orientation = StackOrientation.Horizontal;
                    //Size the image to the textblock, wait until layout is complete (loaded event)
                    uiIcon.Loaded += (sender, e) =>
                    {
                        uiIcon.HeightRequest = Math.Min(uiTitle.Height, (double)actionsConfig.IconSize);
                    };
                }
                contentStackPanel.Children.Add(uiIcon);

                // Add spacing for the icon for horizontal actions
                if (actionsConfig.IconPlacement == IconPlacement.LeftOfTitle)
                {
                    int spacing = context.Config.GetSpacing(AdaptiveSpacing.Default);
                    var uiSep = new Grid
                    {
                        Style = context.GetStyle($"Adaptive.VerticalSeparator"),
                        VerticalOptions = LayoutOptions.Fill,
                        WidthRequest = spacing,
                    };
                    contentStackPanel.Children.Add(uiSep);
                }
            }

            if (!context.IsRenderingSelectAction)
            {
                contentStackPanel.Children.Add(uiTitle);
            }
            else if (!string.IsNullOrEmpty(action.Title))
            {
                //uiButton.ToolTip = uiTitle;
            }

            string name = context.GetType().Name.Replace("Action", String.Empty);
            return uiButton;
        }

    }
}