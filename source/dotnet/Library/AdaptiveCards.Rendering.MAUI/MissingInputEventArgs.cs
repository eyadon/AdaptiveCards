// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace AdaptiveCards.Rendering.MAUI
{
    public class MissingInputEventArgs : EventArgs
    {
        public MissingInputEventArgs(AdaptiveInput input, View frameworkElement)
        {
            this.FrameworkElement = frameworkElement;
            this.AdaptiveInput = input;
        }

        public View FrameworkElement { get; private set; }

        public AdaptiveInput AdaptiveInput { get; private set; }
    }
}
