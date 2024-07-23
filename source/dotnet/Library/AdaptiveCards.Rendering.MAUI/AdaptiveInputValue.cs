// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Text.RegularExpressions;
using AdaptiveCards.Rendering.MAUI.Helpers;

namespace AdaptiveCards.Rendering.MAUI
{
    public abstract class AdaptiveInputValue
    {
        public AdaptiveInputValue(AdaptiveInput input, VisualElement renderedInput)
        {
            InputElement = input;
            RenderedInputElement = renderedInput;
            VisualElementForAccessibility = renderedInput;
        }

        public AdaptiveInputValue(AdaptiveInput input, VisualElement renderedInput, VisualElement visualElementForAccessibility)
        {
            InputElement = input;
            RenderedInputElement = renderedInput;
            VisualElementForAccessibility = visualElementForAccessibility;
        }

        public abstract string GetValue();

        public abstract bool Validate();

        public abstract void SetFocus();

        public virtual void ChangeVisualCueVisibility(bool inputIsValid)
        {
            // Change visibility for error message (and spacing)
            if (ErrorMessage != null)
            {
                TagContent tagContent = TagAttachedProperty.GetTag(ErrorMessage);
                RendererUtil.SetVisibility(ErrorMessage, !inputIsValid, tagContent);

                string helpText = "";
                if (!inputIsValid)
                {
                    helpText = ErrorMessage.Text;
                }

                AutomationProperties.SetHelpText(VisualElementForAccessibility, helpText);
            }
        }

        public AdaptiveInput InputElement { get; set; }

        public VisualElement RenderedInputElement { get; set; }

        public Label ErrorMessage { private get; set; }

        public VisualElement VisualElementForAccessibility { get; set; }
    }

    /// <summary>
    /// Abstract class that implements the Validate behaviour for isRequired in almost all inputValues (except for Input.Toggle)
    /// </summary>
    public abstract class AdaptiveInputValueNonEmptyValidation : AdaptiveInputValue
    {
        public AdaptiveInputValueNonEmptyValidation(AdaptiveInput inputElement, VisualElement renderedElement) : base(inputElement, renderedElement) { }

        public AdaptiveInputValueNonEmptyValidation(AdaptiveInput input, VisualElement renderedInput, VisualElement visualElementForAccessibility) :
            base(input, renderedInput, visualElementForAccessibility)
        { }

        public override bool Validate()
        {
            bool isValid = true;

            if (InputElement.IsRequired)
            {
                isValid = !(String.IsNullOrEmpty(GetValue()));
            }

            return isValid;
        }
    }

    /// <summary>
    /// Intermediate class, as most of the elements in the vanilla wpf (no xceed) renderers use a textbox,
    /// this class was created to avoid all inputValues to implement the same GetValue and Focus method
    /// </summary>
    public class AdaptiveTextBoxInputValue : AdaptiveInputValueNonEmptyValidation
    {
        public AdaptiveTextBoxInputValue(AdaptiveInput inputElement, VisualElement renderedElement) : base(inputElement, renderedElement) { }

        public override string GetValue()
        {
            return (RenderedInputElement as Label).Text;
        }

        public override void SetFocus()
        {
            RenderedInputElement.Focus();
        }

        public override void ChangeVisualCueVisibility(bool isInputValid)
        {
            base.ChangeVisualCueVisibility(isInputValid);

            if (isInputValid)
            {
                VisualErrorCue.StrokeThickness = 0;
            }
            else
            {
                VisualErrorCue.StrokeThickness = 2;
            }
        }

        public Border VisualErrorCue { private get; set; }
    }

    public class AdaptiveTextInputValue : AdaptiveTextBoxInputValue
    {
        public AdaptiveTextInputValue(AdaptiveTextInput inputElement, View renderedElement) : base(inputElement, renderedElement) { }

        public override bool Validate()
        {
            bool isValid = base.Validate();

            AdaptiveTextInput textInput = InputElement as AdaptiveTextInput;

            if (!String.IsNullOrEmpty(textInput.Regex) && !String.IsNullOrEmpty(GetValue()))
            {
                isValid = isValid && Regex.IsMatch(GetValue(), textInput.Regex);
            }

            if (textInput.MaxLength != 0)
            {
                isValid = isValid && (GetValue().Length <= textInput.MaxLength);
            }

            return isValid;
        }
    }

    public class AdaptiveNumberInputValue : AdaptiveTextBoxInputValue
    {
        public AdaptiveNumberInputValue(AdaptiveNumberInput inputElement, View renderedElement) : base(inputElement, renderedElement) { }

        public override bool Validate()
        {
            bool isValid = base.Validate();

            AdaptiveNumberInput numberInput = InputElement as AdaptiveNumberInput;
            double inputValue = 0.0;

            if (isValid && Double.TryParse(GetValue(), out inputValue))
            {


                bool isMinValid = true, isMaxValid = true;
                if (!Double.IsNaN(numberInput.Min))
                {
                    isMinValid = (inputValue >= numberInput.Min);
                }

                if (!Double.IsNaN(numberInput.Max))
                {
                    isMaxValid = (inputValue <= numberInput.Max);
                }

                isValid = isValid && isMinValid && isMaxValid;
            }
            else
            {
                // if the input is not required and the string is empty, then proceed
                // This is a fail safe as non xceed controls are rendered with a Text
                if (!(!numberInput.IsRequired && String.IsNullOrEmpty(GetValue())))
                {
                    isValid = false;
                }
            }

            return isValid;
        }
    }

    public class AdaptiveDateInputValue : AdaptiveTextBoxInputValue
    {
        public AdaptiveDateInputValue(AdaptiveDateInput inputElement, View renderedElement) : base(inputElement, renderedElement) { }

        public override string GetValue()
        {
            return ToIso8601Date(base.GetValue());
        }

        public override bool Validate()
        {
            bool isValid = base.Validate();

            AdaptiveDateInput dateInput = InputElement as AdaptiveDateInput;
            // Check if our input is valid
            string currentValue = GetValue();
            DateTime inputValue;
            if (DateTime.TryParse(currentValue, out inputValue))
            {
                DateTime minDate, maxDate;

                if (!String.IsNullOrEmpty(dateInput.Min))
                {
                    // if min is a valid date, compare against it, otherwise ignore
                    if (DateTime.TryParse(dateInput.Min, out minDate))
                    {
                        isValid = isValid && (inputValue >= minDate);
                    }
                }

                if (!String.IsNullOrEmpty(dateInput.Max))
                {
                    // if max is a valid date, compare against it, otherwise ignore
                    if (DateTime.TryParse(dateInput.Max, out maxDate))
                    {
                        isValid = isValid && (inputValue <= maxDate);
                    }
                }
            }
            else
            {
                // if the input is not required and the string is empty, then proceed
                // This is a fail safe as non xceed controls are rendered with a Text
                if (!(!dateInput.IsRequired && String.IsNullOrEmpty(currentValue)))
                {
                    isValid = false;
                }
            }

            return isValid;
        }

        protected string ToIso8601Date(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            DateTime dateTime;
            if (DateTime.TryParse(text, null, System.Globalization.DateTimeStyles.RoundtripKind, out dateTime))
            {
                return dateTime.ToString("yyyy-MM-dd");
            }

            return text;
        }
    }

    public class AdaptiveTimeInputValue : AdaptiveTextBoxInputValue
    {
        public AdaptiveTimeInputValue(AdaptiveTimeInput inputElement, VisualElement renderedElement) : base(inputElement, renderedElement) { }

        public override string GetValue()
        {
            return ToIso8601Time(base.GetValue());
        }

        public override bool Validate()
        {
            bool isValid = base.Validate();

            AdaptiveTimeInput timeInput = InputElement as AdaptiveTimeInput;
            string currentValue = GetValue();

            // Check if our input is valid
            TimeSpan inputValue;
            if (TimeSpan.TryParse(currentValue, out inputValue))
            {
                TimeSpan minTime, maxTime;

                if (!String.IsNullOrEmpty(timeInput.Min))
                {
                    // if min is a valid date, compare against it, otherwise ignore
                    if (TimeSpan.TryParse(timeInput.Min, out minTime))
                    {
                        isValid = isValid && (inputValue >= minTime);
                    }
                }

                if (!String.IsNullOrEmpty(timeInput.Max))
                {
                    // if max is a valid date, compare against it, otherwise ignore
                    if (TimeSpan.TryParse(timeInput.Max, out maxTime))
                    {
                        isValid = isValid && (inputValue <= maxTime);
                    }
                }
            }
            else
            {
                // if the input is not required and the string is empty, then proceed
                // This is a fail safe as non xceed controls are rendered with a TextBox
                if (!(!timeInput.IsRequired && String.IsNullOrEmpty(currentValue)))
                {
                    isValid = false;
                }
            }

            return isValid;
        }

        protected string ToIso8601Time(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            DateTime dateTime;
            if (DateTime.TryParse(text, null, System.Globalization.DateTimeStyles.RoundtripKind, out dateTime))
            {
                return dateTime.ToString("HH:mm");
            }

            return text;
        }
    }

    public class AdaptiveChoiceSetInputValue : AdaptiveInputValueNonEmptyValidation
    {
        public AdaptiveChoiceSetInputValue(AdaptiveChoiceSetInput inputElement, VisualElement renderedElement) : base(inputElement, renderedElement) { }

        public AdaptiveChoiceSetInputValue(AdaptiveChoiceSetInput inputElement, VisualElement renderedElement, VisualElement uIElement) : base(inputElement, renderedElement, uIElement) { }

        public override string GetValue()
        {
            AdaptiveChoiceSetInput input = InputElement as AdaptiveChoiceSetInput;

            if (input.IsMultiSelect)
            {
                Layout uiChoices = RenderedInputElement as Layout;

                string values = string.Empty;
                foreach (var item in uiChoices.Children)
                {
                    //item is horizontal stack layout [checkbox][label]
                    CheckBox checkBox = (CheckBox)((HorizontalStackLayout)item).Children[0];
                    AdaptiveChoice adaptiveChoice = checkBox.BindingContext as AdaptiveChoice;
                    if (checkBox.IsChecked == true)
                        values += (values == string.Empty ? "" : ",") + adaptiveChoice.Value;
                }
                return values;
            }
            else
            {
                Picker uiComboBox = RenderedInputElement as Picker;

                if (input.Style == AdaptiveChoiceInputStyle.Compact)
                {
                    //TODO this needs some work to return value, not title
                    var item = uiComboBox.SelectedItem as string;
                    if (item != null)
                    {
                        //AdaptiveChoice adaptiveChoice = item.BindingContext as AdaptiveChoice;
                        return item; //adaptiveChoice.Value;
                    }
                    return "";
                }
                else
                {
                    Layout uiChoices = RenderedInputElement as Layout;

                    foreach (var item in uiChoices.Children)
                    {
                        //item is a horizontalStackLayout [button][label]
                        RadioButton radioBox = (RadioButton)((HorizontalStackLayout)item).Children[0];
                        AdaptiveChoice adaptiveChoice = radioBox.BindingContext as AdaptiveChoice;
                        if (radioBox.IsChecked == true)
                            return adaptiveChoice.Value;
                    }
                    return "";
                }
            }
        }

        public override void SetFocus()
        {
            // For expanded cases, the inputs are inserted into a panel,
            // so we focus on the first element in the panel
            if (RenderedInputElement is Layout)
            {
                Layout choicesContainer = RenderedInputElement as Layout;

                if (choicesContainer.Children.Count > 0)
                {
                    choicesContainer.Children[0].Focus();
                }
            }
            else
            {
                RenderedInputElement.Focus();
            }
        }
    }

    public class AdaptiveToggleInputValue : AdaptiveInputValue
    {
        public AdaptiveToggleInputValue(AdaptiveToggleInput inputElement, VisualElement renderedElement) : base(inputElement, renderedElement) { }

        public override string GetValue()
        {
            CheckBox uiToggle = RenderedInputElement as CheckBox;
            AdaptiveToggleInput toggleInput = InputElement as AdaptiveToggleInput;

            return (uiToggle.GetState() == true ? toggleInput.ValueOn ?? "true" : toggleInput.ValueOff ?? "false");
        }

        public override void SetFocus()
        {
            RenderedInputElement.Focus();
        }

        public override bool Validate()
        {
            bool isValid = true;

            if (InputElement.IsRequired)
            {
                AdaptiveToggleInput toggleInput = InputElement as AdaptiveToggleInput;
                isValid = (GetValue() == toggleInput.ValueOn);
            }

            return isValid;
        }
    }
}
