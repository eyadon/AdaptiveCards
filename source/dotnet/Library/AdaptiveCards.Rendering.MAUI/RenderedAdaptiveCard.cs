// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace AdaptiveCards.Rendering.MAUI
{
    public class RenderedAdaptiveCard : RenderedAdaptiveCardBase
    {
        public RenderedAdaptiveCard(
            View frameworkElement,
            AdaptiveCard originatingCard,
            IList<AdaptiveWarning> warnings,
            ref IDictionary<string, Func<string>> inputBindings)
            : base(originatingCard, warnings)
        {
            FrameworkElement = frameworkElement;
            UserInputs = new RenderedAdaptiveCardInputs(ref inputBindings);
        }

        /// <summary>
        /// The rendered card
        /// </summary>
        public View FrameworkElement { get; }

        /// <summary>
        /// Event handler for when user invokes an action.
        /// </summary>
        public event TypedEventHandler<RenderedAdaptiveCard, AdaptiveActionEventArgs> OnAction;

        internal void InvokeOnAction(AdaptiveActionEventArgs args)
        {
            OnAction?.Invoke(this, args);
        }

        /// <summary>
        /// Event handler for when user clicks a media.
        /// </summary>
        public event TypedEventHandler<RenderedAdaptiveCard, AdaptiveMediaEventArgs> OnMediaClicked;

        internal void InvokeOnMediaClick(AdaptiveMediaEventArgs args)
        {
            OnMediaClicked?.Invoke(this, args);
        }
    }
}
