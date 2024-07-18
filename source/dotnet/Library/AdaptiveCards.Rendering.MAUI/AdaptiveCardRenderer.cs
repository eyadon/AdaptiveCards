// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Diagnostics;
using AdaptiveCards.Rendering.MAUI.Helpers;

namespace AdaptiveCards.Rendering.MAUI
{
    public class AdaptiveCardRenderer : AdaptiveCardRendererBase<View, AdaptiveRenderContext>
    {
        protected override AdaptiveSchemaVersion GetSupportedSchemaVersion()
        {
            return new AdaptiveSchemaVersion(1, 5);
        }

        protected Action<object, AdaptiveActionEventArgs> ActionCallback;
        protected Action<object, MissingInputEventArgs> missingDataCallback;

        public AdaptiveCardRenderer() : this(new AdaptiveHostConfig()) { }

        public AdaptiveCardRenderer(AdaptiveHostConfig hostConfig)
        {
            HostConfig = hostConfig;
            SetObjectTypes();
        }

        private void SetObjectTypes()
        {
            ElementRenderers.Set<AdaptiveCard>(RenderAdaptiveCardWrapper);

            ElementRenderers.Set<AdaptiveTextBlock>(AdaptiveTextBlockRenderer.Render);
            //ElementRenderers.Set<AdaptiveRichTextBlock>(AdaptiveRichTextBlockRenderer.Render);

            //ElementRenderers.Set<AdaptiveImage>(AdaptiveImageRenderer.Render);
            //We can fix this up if we ever need to use it...
            //ElementRenderers.Set<AdaptiveMedia>(AdaptiveMediaRenderer.Render);

            ElementRenderers.Set<AdaptiveContainer>(AdaptiveContainerRenderer.Render);
            ElementRenderers.Set<AdaptiveColumn>(AdaptiveColumnRenderer.Render);
            ElementRenderers.Set<AdaptiveColumnSet>(AdaptiveColumnSetRenderer.Render);
            ElementRenderers.Set<AdaptiveFactSet>(AdaptiveFactSetRenderer.Render);
            //ElementRenderers.Set<AdaptiveImageSet>(AdaptiveImageSetRenderer.Render);
            ElementRenderers.Set<AdaptiveActionSet>(AdaptiveActionSetRenderer.Render);

            ElementRenderers.Set<AdaptiveChoiceSetInput>(AdaptiveChoiceSetRenderer.Render);
            ElementRenderers.Set<AdaptiveTextInput>(AdaptiveTextInputRenderer.Render);
            ElementRenderers.Set<AdaptiveNumberInput>(AdaptiveNumberInputRenderer.Render);
            ElementRenderers.Set<AdaptiveDateInput>(AdaptiveDateInputRenderer.Render);
            ElementRenderers.Set<AdaptiveTimeInput>(AdaptiveTimeInputRenderer.Render);
            ElementRenderers.Set<AdaptiveToggleInput>(AdaptiveToggleInputRenderer.Render);

            ElementRenderers.Set<AdaptiveAction>(AdaptiveActionRenderer.Render);
        }

        public AdaptiveFeatureRegistration FeatureRegistration { get; } = new AdaptiveFeatureRegistration();

        /// <summary>
        /// A path to a XAML resource dictionary
        /// </summary>
        public string ResourcesPath { get; set; }

        private ResourceDictionary _resources;

        /// <summary>
        /// Resource dictionary to use when rendering. Don't use this from a server, use <see cref="ResourcesPath"/> instead.
        /// </summary>
        public ResourceDictionary Resources
        {
            get
            {
                if (_resources != null)
                    return _resources;

                if (File.Exists(ResourcesPath))
                {
                    using (var styleStream = File.OpenRead(ResourcesPath))
                    {
                        var reader = new StreamReader(styleStream);
                        var content = reader.ReadToEnd();
                        _resources = new ResourceDictionary().LoadFromXaml(content);
                        //_resources = (ResourceDictionary)XamlReader.Load(styleStream);
                    }
                }
                else
                {
                    _resources = new ResourceDictionary();
                }

                // Wrap this to avoid Console applications to crash because of this : https://github.com/Microsoft/AdaptiveCards/issues/2121
                try
                {
                    var resource = new ResourceDictionary
                    {
                        Source = new Uri("/AdaptiveCards.Rendering.MAUI;component/Themes/generic.xaml",
                       UriKind.RelativeOrAbsolute)
                    };
                    _resources.MergedDictionaries.Add(resource);
                }
                catch { }

                return _resources;
            }
            set
            {
                _resources = value;

                // Wrap this to avoid Console applications to crash because of this : https://github.com/Microsoft/AdaptiveCards/issues/2121
                //try
                //{
                //    var resource = new ResourceDictionary
                //    {
                //        Source = new Uri("/AdaptiveCards.Rendering.Wpf;component/Themes/generic.xaml", UriKind.RelativeOrAbsolute)
                //    };
                //    _resources.MergedDictionaries.Add(resource);
                //}
                //catch { }

            }

        }

        public AdaptiveActionHandlers ActionHandlers { get; } = new AdaptiveActionHandlers();

        public ResourceResolver ResourceResolvers { get; } = new ResourceResolver();

        public static View RenderAdaptiveCardWrapper(AdaptiveCard card, AdaptiveRenderContext context)
        {
            var outerGrid = new Grid();
            outerGrid.Style = context.GetStyle("Adaptive.Card");

            outerGrid.Background = context.GetColorBrush(context.Config.ContainerStyles.Default.BackgroundColor);
            //TODO IMG outerGrid.SetBackgroundSource(card.BackgroundImage, context);

            if(context.CardRoot == null)
            {
                context.CardRoot = outerGrid;
            }

            // Reset the parent style
            context.RenderArgs.ParentStyle = AdaptiveContainerStyle.Default;

            var grid = new Grid();
            grid.Style = context.GetStyle("Adaptive.InnerCard");
            grid.Margin = new Thickness(context.Config.Spacing.Padding);

            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

            switch (card.VerticalContentAlignment)
            {
                case AdaptiveVerticalContentAlignment.Center:
                    grid.VerticalOptions = LayoutOptions.Center;
                    break;
                case AdaptiveVerticalContentAlignment.Bottom:
                    grid.VerticalOptions = LayoutOptions.End;
                    break;
                case AdaptiveVerticalContentAlignment.Top:
                default:
                    break;
            }

            outerGrid.MinimumHeightRequest = card.PixelMinHeight;

            outerGrid.Children.Add(grid);

            AdaptiveInternalID parentCardId = context.RenderArgs.ContainerCardId;
            context.ParentCards.Add(card.InternalID, parentCardId);
            context.RenderArgs.ContainerCardId = card.InternalID;

            AdaptiveContainerRenderer.AddContainerElements(grid, card.Body, context);
            AdaptiveActionSetRenderer.AddRenderedActions(grid, card.Actions, context, card.InternalID);

            context.RenderArgs.ContainerCardId = parentCardId;

            if (card.SelectAction != null)
            {
                var outerGridWithSelectAction = context.RenderSelectAction(card.SelectAction, outerGrid);

                return outerGridWithSelectAction;
            }

            return outerGrid;
        }

        /// <summary>
        /// Renders an adaptive card.
        /// </summary>
        /// <param name="card">The card to render</param>
        public RenderedAdaptiveCard RenderCard(AdaptiveCard card)
        {
            if (card == null) throw new ArgumentNullException(nameof(card));
            RenderedAdaptiveCard renderCard = null;

            void ActionCallback(object sender, AdaptiveActionEventArgs args)
            {
                renderCard?.InvokeOnAction(args);
            }

            void MediaClickCallback(object sender, AdaptiveMediaEventArgs args)
            {
                renderCard?.InvokeOnMediaClick(args);
            }

            var context = new AdaptiveRenderContext(ActionCallback, null, MediaClickCallback)
            {
                ResourceResolvers = ResourceResolvers,
                ActionHandlers = ActionHandlers,
                Config = HostConfig ?? new AdaptiveHostConfig(),
                Resources = Resources,
                ElementRenderers = ElementRenderers,
                FeatureRegistration = FeatureRegistration,
                Lang = card.Lang,
                RenderArgs = new AdaptiveRenderArgs { ForegroundColors = (HostConfig != null) ? HostConfig.ContainerStyles.Default.ForegroundColors : new ContainerStylesConfig().Default.ForegroundColors }
            };

            string accentColor = HostConfig.ContainerStyles.Default.ForegroundColors.Accent.Default;
            string lighterAccentColor = ColorUtil.GenerateLighterColor(accentColor);
            string attentionColor = HostConfig.ContainerStyles.Default.ForegroundColors.Attention.Default;
            string lighterAttentionColor = ColorUtil.GenerateLighterColor(attentionColor);

            Resources["Adaptive.Action.Positive.Button.Static.Background"] = context.GetColorBrush(accentColor);
            Resources["Adaptive.Action.Positive.Button.MouseOver.Background"] = context.GetColorBrush(lighterAccentColor);
            Resources["Adaptive.Action.Destructive.Button.Foreground"] = context.GetColorBrush(attentionColor);
            Resources["Adaptive.Action.Destructive.Button.MouseOver.Foreground"] = context.GetColorBrush(lighterAttentionColor);

            var element = context.Render(card);

            renderCard = new RenderedAdaptiveCard(element, card, context.Warnings, ref context.InputBindings);

            return renderCard;
        }

        /// <summary>
        /// Renders an adaptive card to a PNG image. This method cannot be called from a server. Use <see cref="RenderCardToImageOnStaThreadAsync"/> instead.
        /// </summary>
        /// <param name="createStaThread">If true this method will create an STA thread allowing it to be called from a server.</param>
        //public async Task<RenderedAdaptiveCardImage> RenderCardToImageAsync(AdaptiveCard card, bool createStaThread, int width = 400, CancellationToken cancellationToken = default(CancellationToken))
        //{
        //    if (card == null) throw new ArgumentNullException(nameof(card));

        //    if (createStaThread)
        //    {
        //        return await await Task.Factory.StartNewSta(async () => await RenderCardToImageInternalAsync(card, width, cancellationToken));
        //    }
        //    else
        //    {
        //        return await RenderCardToImageInternalAsync(card, width, cancellationToken);
        //    }
        //}

        //private async Task<RenderedAdaptiveCardImage> RenderCardToImageInternalAsync(AdaptiveCard card, int width, CancellationToken cancellationToken)
        //{
        //    RenderedAdaptiveCardImage renderCard = null;

        //    try
        //    {
        //        var cardAssets = await LoadAssetsForCardAsync(card, cancellationToken);

        //        var context = new AdaptiveRenderContext(null, null, null)
        //        {
        //            CardAssets = cardAssets,
        //            ResourceResolvers = ResourceResolvers,
        //            ActionHandlers = ActionHandlers,
        //            Config = HostConfig ?? new AdaptiveHostConfig(),
        //            Resources = Resources,
        //            ElementRenderers = ElementRenderers,
        //            Lang = card.Lang,
        //            RenderArgs = new AdaptiveRenderArgs { ForegroundColors = (HostConfig != null) ? HostConfig.ContainerStyles.Default.ForegroundColors : new ContainerStylesConfig().Default.ForegroundColors }
        //        };

        //        var stream = context.Render(card).RenderToImage(width);
        //        renderCard = new RenderedAdaptiveCardImage(stream, card, context.Warnings);
        //    }
        //    catch (Exception e)
        //    {
        //        Debug.WriteLine($"RENDER Failed. {e.Message}");
        //    }

        //    return renderCard;
        //}


        public async Task<IDictionary<Uri, MemoryStream>> LoadAssetsForCardAsync(AdaptiveCard card, CancellationToken cancellationToken = default(CancellationToken))
        {
            var visitor = new PreFetchImageVisitor(ResourceResolvers);
            await visitor.GetAllImages(card).WithCancellation(cancellationToken).ConfigureAwait(false);
            return visitor.LoadedImages;
        }
    }
}