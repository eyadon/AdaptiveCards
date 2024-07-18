// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using AdaptiveCards.Rendering.MAUI.Helpers;

namespace AdaptiveCards.Rendering.MAUI
{
    public static class AdaptiveDateInputRenderer
    {
        public static View Render(AdaptiveDateInput input, AdaptiveRenderContext context)
        {
            var textBox = new Entry() { Text = input.Value };
            textBox.SetPlaceholder(input.Placeholder);
            textBox.Style = context.GetStyle($"Adaptive.Input.Text.Date");
            textBox.SetContext(input);

            DateTime maxDate, minDate;
            if ((DateTime.TryParse(input.Max, out maxDate) || DateTime.TryParse(input.Min, out minDate) || input.IsRequired)
                && string.IsNullOrEmpty(input.ErrorMessage))
            {
                context.Warnings.Add(new AdaptiveWarning((int)AdaptiveWarning.WarningStatusCode.NoErrorMessageForValidatedInput,
                    "Inputs with validation should include an ErrorMessage"));
            }

            context.InputValues.Add(input.Id, new AdaptiveDateInputValue(input, textBox));

            return textBox;
        }
    }
}
