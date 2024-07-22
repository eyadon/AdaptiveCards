// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace AdaptiveCards.Rendering.MAUI
{
    public class ResourceResolver
    {
        private readonly Dictionary<string, Func<Uri, Task<Stream>>> _internalResolver = new Dictionary<string, Func<Uri, Task<Stream>>>(StringComparer.OrdinalIgnoreCase);

        public ResourceResolver()
        {
            //Register("http", GetHttpAsync);
            //Register("https", GetHttpAsync);
            //Register("pack", GetPackAsync);
            Register("data", GetDataUriAsync);
        }

        //private static async Task<MemoryStream> GetHttpAsync(Uri uri)
        //{
        //    using (var webclient = new WebClient())
        //    {
        //        webclient.CachePolicy = new RequestCachePolicy(RequestCacheLevel.CacheIfAvailable);
        //        var bytes = await webclient.DownloadDataTaskAsync(uri);
        //        return new MemoryStream(bytes);
        //    }
        //}

        /* Helper function to get stream from either Resource or Content */
        //private static StreamResourceInfo GetResourceOrContentStream(Uri uri)
        //{
        //    StreamResourceInfo info = Application.GetResourceStream(uri);
        //    if (info == null)
        //    {
        //        info = Application.GetContentStream(uri);
        //    }

        //    return info;
        //}

        //private static Task<MemoryStream> GetPackAsync(Uri uri)
        //{
        //    try
        //    {
        //        var info = GetResourceOrContentStream(uri);
        //        if (info == null)
        //        {
        //            throw new IOException("Unable to locate pack URI");
        //        }

        //        var memoryStream = new MemoryStream();
        //        info.Stream.CopyTo(memoryStream);
        //        return Task.FromResult(memoryStream);
        //    }
        //    catch (Exception)
        //    {
        //        return Task.FromResult<MemoryStream>(null);
        //    }
        //}

        private static Task<Stream> GetDataUriAsync(Uri uri)
        {
            try
            {
                var encodedData = uri.AbsoluteUri.Substring(uri.AbsoluteUri.LastIndexOf(',') + 1);

                var decodedDataUri = Convert.FromBase64String(encodedData);
                var memoryStream = new MemoryStream(decodedDataUri);

                return Task.FromResult(memoryStream as Stream);
            }
            catch (Exception)
            {
                return Task.FromResult<Stream>(null);
            }
        }

        public void Register(string uriScheme, Func<Uri, Task<Stream>> loadAsset)
        {
            _internalResolver[uriScheme] = loadAsset;
        }

        public void Clear()
        {
            _internalResolver.Clear();
        }

        public void Remove(string uriScheme)
        {
            _internalResolver.Remove(uriScheme);
        }

        public Task<Stream> LoadAssetAsync(Uri uri)
        {
            if (uri == null) throw new ArgumentNullException(nameof(uri));

            if (_internalResolver.TryGetValue(uri.Scheme, out var func))
                return func(uri);

            // TODO: Context warning, no asset resolver for URI scheme
            return Task.FromResult<Stream>(null);
        }
    }
}
