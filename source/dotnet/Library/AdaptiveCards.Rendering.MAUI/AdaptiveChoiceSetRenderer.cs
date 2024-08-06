// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace AdaptiveCards.Rendering.MAUI
{
    public static class AdaptiveChoiceSetRenderer
    {
        public static View Render(AdaptiveChoiceSetInput input, AdaptiveRenderContext context)
        {
            return RenderHelper(new Grid(), new Picker(), new StackLayout(), input, context);
        }

        public static View RenderHelper(Grid uiGrid, Picker uiComboBox, StackLayout uiChoices, AdaptiveChoiceSetInput input, AdaptiveRenderContext context)
        {
            var chosen = input.Value?.Split(',').Select(p => p.Trim()).Where(s => !string.IsNullOrEmpty(s)).ToList() ?? new List<string>();

            uiGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            uiGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });

            uiComboBox.Style = context.GetStyle("Adaptive.Input.AdaptiveChoiceSetInput.ComboBox");
            uiComboBox.BindingContext = input;
            uiComboBox.Title = input.Placeholder;
            var dropdownItems = new List<AdaptiveChoice>();
            uiComboBox.ItemDisplayBinding = new Binding("Title");
            
            uiChoices.BindingContext = input;
            uiChoices.Style = context.GetStyle("Adaptive.Input.AdaptiveChoiceSetInput");

            foreach (var choice in input.Choices)
            {

                if (input.IsMultiSelect == true)
                {
                    var container = new HorizontalStackLayout();
                    var label = new Label();
                    var uiCheckbox = new CheckBox();
                    container.Children.Add(uiCheckbox);
                    container.Children.Add(label);
                    SetContent(label, choice.Title, input.Wrap);
                    uiCheckbox.IsChecked = chosen.Contains(choice.Value);
                    uiCheckbox.BindingContext = choice;
                    uiCheckbox.Style = context.GetStyle("Adaptive.Input.AdaptiveChoiceSetInput.CheckBox");
                    label.Style = context.GetStyle("Adaptive.Input.AdaptiveChoiceSetInput.Label");
                    uiChoices.Children.Add(container);
                }
                else
                {
                    if (input.Style == AdaptiveChoiceInputStyle.Compact)
                    {
                        //var uiComboItem = new ComboBoxItem();
                        //uiComboItem.HorizontalAlignment = HorizontalAlignment.Stretch;
                        //uiComboItem.Style = context.GetStyle("Adaptive.Input.AdaptiveChoiceSetInput.ComboBoxItem");

                        //Label content = SetContent(uiComboItem, choice.Title, input.Wrap);
                        //// The content TextBlock is binded to the width of the comboBox container
                        //if (input.Wrap && content != null)
                        //{
                        //    content.SetBinding(Label.MaximumWidthRequestProperty,
                        //        new Binding("Width") { Source = uiComboBox });
                        //}

                        //uiComboItem.BindingContext = choice;
                        
                        dropdownItems.Add(choice);


                    }
                    else
                    {
                        var container = new HorizontalStackLayout();
                        var label = new Label();
                        var uiRadio = new RadioButton();
                        container.Children.Add(uiRadio);
                        container.Children.Add(label);
                        SetContent(label, choice.Title, input.Wrap);

                        // When isMultiSelect is false, only 1 specified value is accepted.
                        // Otherwise, don't set any option
                        if (chosen.Count == 1)
                        {
                            uiRadio.IsChecked = chosen.Contains(choice.Value);
                        }
                        uiRadio.GroupName = input.Id;
                        uiRadio.BindingContext = choice;
                        uiRadio.Style = context.GetStyle("Adaptive.Input.AdaptiveChoiceSetInput.Radio");
                        label.Style = context.GetStyle("Adaptive.Input.AdaptiveChoiceSetInput.Label");
                        uiChoices.Children.Add(container);
                    }
                }
            }

            uiComboBox.ItemsSource = dropdownItems;
            // If multiple values are specified, no option is selected
            if (chosen.Count == 1)
            {
                uiComboBox.SelectedItem = dropdownItems.FirstOrDefault(i => i.Value == chosen.First());
            }

            AdaptiveChoiceSetInputValue inputValue = null;
            
            if (!input.IsMultiSelect && input.Style == AdaptiveChoiceInputStyle.Compact)
            {
                Grid.SetRow(uiComboBox, 1);
                uiGrid.Children.Add(uiComboBox);
                inputValue = new AdaptiveChoiceSetInputValue(input, uiComboBox);
            }
            else
            {
                Grid.SetRow(uiChoices, 1);
                uiGrid.Children.Add(uiChoices);
                inputValue = new AdaptiveChoiceSetInputValue(input, uiChoices, (View)uiChoices.Children[0]);
            }

            if (input.IsRequired && string.IsNullOrEmpty(input.ErrorMessage))
            {
                context.Warnings.Add(new AdaptiveWarning((int)AdaptiveWarning.WarningStatusCode.NoErrorMessageForValidatedInput,
                    "Inputs with validation should include an ErrorMessage"));
            }

            context.InputValues.Add(input.Id, inputValue);

            return uiGrid;
        }

        public static Label SetContent(Label uiControl, string text, bool wrap)
        {
            uiControl.Text = text;
            if (wrap)
            {
                uiControl.LineBreakMode = LineBreakMode.WordWrap;
            }
            return uiControl;
        }
    }
}
