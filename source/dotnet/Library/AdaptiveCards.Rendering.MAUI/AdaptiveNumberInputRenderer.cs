// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using AdaptiveCards.Rendering.MAUI.Helpers;

namespace AdaptiveCards.Rendering.MAUI
{

    public static class AdaptiveNumberInputRenderer
    {
        public static View Render(AdaptiveNumberInput input, AdaptiveRenderContext context)
        {
            var textBox = new Entry();

            if (!Double.IsNaN(input.Value))
            {
                textBox.Text = input.Value.ToString();
            }
            textBox.SetPlaceholder(input.Placeholder);
            textBox.Style = context.GetStyle($"Adaptive.Input.Text.Number");
            textBox.SetContext(input);

            if ((!Double.IsNaN(input.Max) || !Double.IsNaN(input.Min) || input.IsRequired)
                && string.IsNullOrEmpty(input.ErrorMessage))
            {
                context.Warnings.Add(new AdaptiveWarning((int)AdaptiveWarning.WarningStatusCode.NoErrorMessageForValidatedInput,
                    "Inputs with validation should include an ErrorMessage"));
            }

            context.InputValues.Add(input.Id, new AdaptiveNumberInputValue(input, textBox));

            return textBox;
        }
    }
}
