using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RestLibrary.Extensions
{
    internal static class HttpClientExtensions
    {
        public static async Task<HttpResponseMessage> PostAsJsonAsync<T>(this HttpClient client, string resource, T value)
        {
            var content = GetStringContent(value);
            var response = await client.PostAsync(resource, content).ConfigureAwait(false);

            return response;
        }

        public static async Task<HttpResponseMessage> PutAsJsonAsync<T>(this HttpClient client, string resource, T value)
        {
            var content = GetStringContent(value);
            var response = await client.PutAsync(resource, content).ConfigureAwait(false);

            return response;
        }

        private static StringContent GetStringContent<T>(T obj)
        {
            var stringData = typeof(T) == typeof(string) ? obj?.ToString() : obj.ToJson();
            var content = new StringContent(stringData, Encoding.UTF8, RestClient.JsonMimeType);
            return content;
        }
    }
}
