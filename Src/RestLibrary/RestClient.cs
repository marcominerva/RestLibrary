using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using RestLibrary.Models;
using Newtonsoft.Json.Linq;
using RestLibrary.Extensions;

namespace RestLibrary
{
    public class RestClient : IRestClient
    {
        internal const string JsonMimeType = "application/json";
        internal const string FormUrlEncoded = "application/x-www-form-urlencoded";

        private readonly HttpClient client;
        private Credentials credentials;

        public RestClient(string serviceUrl = null, string language = null)
        {
            var handler = new HttpClientHandler { AllowAutoRedirect = true };
            if (handler.SupportsAutomaticDecompression)
                handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            client = new HttpClient(handler);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(JsonMimeType));

            Language = language;
            BaseAddress = serviceUrl;
        }

        private string baseAddress;
        public string BaseAddress
        {
            get { return baseAddress; }
            set
            {
                value = !string.IsNullOrWhiteSpace(value) ? value : null;

                if (baseAddress != value)
                {
                    baseAddress = value;
                    if (baseAddress != null)
                    {
                        if (!baseAddress.EndsWith("/"))
                            baseAddress += "/";

                        client.BaseAddress = new Uri(baseAddress);
                    }
                    else
                    {
                        client.BaseAddress = null;
                    }
                }
            }
        }

        private string language;
        public string Language
        {
            get { return language; }
            set
            {
                value = !string.IsNullOrWhiteSpace(value) ? value : null;

                if (language != value)
                {
                    language = value;
                    client.DefaultRequestHeaders.Accept.Clear();

                    if (language != null)
                        client.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue(language));
                }
            }
        }

        private string accessToken;
        public string AccessToken
        {
            get
            {
                return accessToken;
            }
            set
            {
                if (accessToken != value)
                {
                    accessToken = value;
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                }
            }
        }

        public HttpRequestHeaders Headers => client.DefaultRequestHeaders;

        public TimeSpan Timeout
        {
            get { return client.Timeout; }
            set { client.Timeout = value; }
        }

        public Task<RestResponse<string>> GetAsync(string resource) => this.GetAsync<string>(resource);

        public async Task<RestResponse<T>> GetAsync<T>(string resource)
        {
            using (var response = await this.InvokeResourceAsync<object>((arg, obj) => client.GetAsync(arg), resource, null))
            {
                var result = await this.DeserializeAsync<T>(response);
                return new RestResponse<T>(response, result);
            }
        }

        public Task<RestResponse> PostAsync(string resource) => this.PostAsync<object>(resource, null);

        public async Task<RestResponse> PostAsync<T>(string resource, T content)
        {
            using (var response = await this.InvokeResourceAsync((arg, obj) => client.PostAsJsonAsync(arg, obj), resource, content))
                return new RestResponse(response);
        }

        public Task<RestResponse<T>> PostWithResultAsync<T>(string resource) => this.PostWithResultAsync<object, T>(resource, null);

        public async Task<RestResponse<U>> PostWithResultAsync<T, U>(string resource, T content)
        {
            using (var response = await this.InvokeResourceAsync((arg, obj) => client.PostAsJsonAsync(arg, obj), resource, content))
            {
                var result = await this.DeserializeAsync<U>(response);
                return new RestResponse<U>(response, result);
            }
        }

        public Task<RestResponse> PutAsync(string resource) => this.PutAsync<object>(resource, null);

        public async Task<RestResponse> PutAsync<T>(string resource, T content)
        {
            using (var response = await this.InvokeResourceAsync((arg, obj) => client.PutAsJsonAsync(arg, obj), resource, content))
                return new RestResponse(response);
        }

        public Task<RestResponse<T>> PutWithResultAsync<T>(string resource) => this.PutWithResultAsync<object, T>(resource, null);

        public async Task<RestResponse<U>> PutWithResultAsync<T, U>(string resource, T content)
        {
            using (var response = await this.InvokeResourceAsync((arg, obj) => client.PutAsJsonAsync(arg, obj), resource, content))
            {
                var result = await this.DeserializeAsync<U>(response);
                return new RestResponse<U>(response, result);
            }
        }

        public async Task<RestResponse> DeleteAsync(string resource)
        {
            using (var response = await this.InvokeResourceAsync<object>((arg, obj) => client.DeleteAsync(arg), resource, null))
                return new RestResponse(response);
        }

        private async Task<HttpResponseMessage> InvokeResourceAsync<T>(Func<string, T, Task<HttpResponseMessage>> action, string resource, T obj)
        {
            resource = this.NormalizeUrl(resource);
            var response = await action(resource, obj).ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.Unauthorized && credentials != null)
            {
                await this.OAuthLoginAsync(credentials.UserName, credentials.Password);
                response = await action.Invoke(resource, obj).ConfigureAwait(false);
            }

            return response;
        }

        private async Task<T> DeserializeAsync<T>(HttpResponseMessage response)
        {
            T result = default(T);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                if (typeof(T) == typeof(string))
                {
                    result = (T)Convert.ChangeType(content, typeof(string));
                }
                else
                {
                    result = JsonConvert.DeserializeObject<T>(content);
                }
            }

            return result;
        }

        public async Task<bool> OAuthLoginAsync(string userName, string password, string path = "token")
        {
            var body = $"grant_type=password&username={userName}&password={password}";
            var data = new StringContent(body, Encoding.UTF8, FormUrlEncoded);
            var response = await client.PostAsync(path, data).ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                // Invalid username or password.
                return false;
            }

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            AccessToken = JToken.Parse(content)["access_token"].ToString();
            credentials = new Credentials { UserName = userName, Password = password };

            return true;
        }

        public Task LogoutAsync()
        {
            AccessToken = null;
            credentials = null;

            return Task.FromResult(true);
        }

        public void Dispose()
        {
            client.Dispose();
            GC.SuppressFinalize(this);
        }

        private string NormalizeUrl(string url) => url?.TrimStart('/');
    }
}
