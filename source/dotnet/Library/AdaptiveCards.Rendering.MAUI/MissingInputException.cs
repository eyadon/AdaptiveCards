// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace AdaptiveCards.Rendering.MAUI
{
    public class MissingInputException : Exception
    {
        public MissingInputException(string message, AdaptiveInput input, View frameworkElement)
            : base(message)
        {
            this.FrameworkElement = frameworkElement;
            this.AdaptiveInput = input;
        }

        public View FrameworkElement { get; set; }

        public AdaptiveInput AdaptiveInput { get; set; }
    }
}
