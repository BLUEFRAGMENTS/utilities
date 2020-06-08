using System;
using System.Net.Http;

namespace Bluefragments.Utilities.Http
{
    public static class HttpClientHelper
    {
        private static Lazy<HttpClient> lazyClient = new Lazy<HttpClient>(InitializeHttpClient);

        public static HttpClient HttpClient => lazyClient.Value;

        private static HttpClient InitializeHttpClient()
        {
            return new HttpClient();
        }
    }
}
