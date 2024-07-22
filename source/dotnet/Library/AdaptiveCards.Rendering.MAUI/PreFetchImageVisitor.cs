// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Concurrent;

namespace AdaptiveCards.Rendering.MAUI
{
    /// <summary>
    /// Walk tree and fetch any images referenced
    /// </summary>
    internal class PreFetchImageVisitor : AdaptiveVisitor
    {
        private readonly ResourceResolver _resourceResolver;
        private readonly List<Task> _tasks = new List<Task>();

        public ConcurrentDictionary<Uri, Stream> LoadedImages { get; } = new ConcurrentDictionary<Uri, Stream>();

        public PreFetchImageVisitor(ResourceResolver resourceResolver)
        {
            _resourceResolver = resourceResolver;
        }

        /// <summary>
        /// Get all images
        /// </summary>
        public async Task GetAllImages(AdaptiveCard card)
        {
            Visit(card);

            await Task.WhenAll(_tasks.ToArray()).ConfigureAwait(false);
        }

        public Stream GetCachedImageStream(Uri url)
        {
            if (LoadedImages.TryGetValue(url, out var stream))
            {
                return stream;
            }
            return null;
        }

        protected async Task GetImage(Uri url)
        {
            LoadedImages[url] = await _resourceResolver.LoadAssetAsync(url).ConfigureAwait(false);
        }

        public override void Visit(AdaptiveCard card)
        {
            if (card.BackgroundImage != null)
                _tasks.Add(GetImage(card.BackgroundImage.Url));
            base.Visit(card);
        }

        public override void Visit(AdaptiveImage image)
        {
            _tasks.Add(GetImage(image.Url));
            base.Visit(image);
        }
    }
}
