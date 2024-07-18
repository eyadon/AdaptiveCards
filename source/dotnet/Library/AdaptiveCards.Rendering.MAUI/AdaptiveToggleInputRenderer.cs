// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using AdaptiveCards.Rendering.MAUI.Helpers;

namespace AdaptiveCards.Rendering.MAUI
{
    public static class AdaptiveToggleInputRenderer
    {
        public static View Render(AdaptiveToggleInput input, AdaptiveRenderContext context)
        {
            var container = new HorizontalStackLayout();
            var label = new Label();
            var uiToggle = new CheckBox();
            container.Children.Add(uiToggle);
            container.Children.Add(label);
            AdaptiveChoiceSetRenderer.SetContent(label, input.Title, input.Wrap);
            uiToggle.Color =
                context.GetColorBrush(context.Config.ContainerStyles.Default.ForegroundColors.Default.Default).Color;
            uiToggle.SetState(input.Value == (input.ValueOn ?? "true"));
            uiToggle.Style = context.GetStyle($"Adaptive.Input.Toggle");
            uiToggle.SetContext(input);

            if (input.IsRequired && string.IsNullOrEmpty(input.ErrorMessage))
            {
                context.Warnings.Add(new AdaptiveWarning((int)AdaptiveWarning.WarningStatusCode.NoErrorMessageForValidatedInput,
                    "Inputs with validation should include an ErrorMessage"));
            }

            context.InputValues.Add(input.Id, new AdaptiveToggleInputValue(input, uiToggle));

            return container;
        }
    }

}
