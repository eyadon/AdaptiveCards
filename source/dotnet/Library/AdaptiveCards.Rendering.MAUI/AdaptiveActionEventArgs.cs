// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace AdaptiveCards.Rendering.MAUI
{
    public class AdaptiveActionEventArgs : EventArgs
    {
        public AdaptiveActionEventArgs(AdaptiveAction action)
        {
            Action = action;
        }

        /// <summary>
        /// The action that fired
        /// </summary>
        public AdaptiveAction Action { get; set; }
    }
}
