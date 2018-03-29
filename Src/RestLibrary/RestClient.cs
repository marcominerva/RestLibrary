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
        internal const string Bearer = "Bearer";

        public RestClient() : this(null, null)
        {
        }

        public RestClient(string baseAddress) : this(baseAddress, null)
        {
        }

        public RestClient(string baseAddress, string language)
        {
            var handler = new HttpClientHandler { AllowAutoRedirect = true };
            if (handler.SupportsAutomaticDecompression)
            {
                handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            }

            HttpClient = new HttpClient(handler);
            HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(JsonMimeType));

            BaseAddress = baseAddress;
            Language = language;
        }

        public HttpClient HttpClient { get; }

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
                        {
                            baseAddress += "/";
                        }

                        HttpClient.BaseAddress = new Uri(baseAddress);
                    }
                    else
                    {
                        HttpClient.BaseAddress = null;
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
                    HttpClient.DefaultRequestHeaders.AcceptLanguage.Clear();

                    if (language != null)
                    {
                        HttpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue(language));
                    }
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
                    HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Bearer, accessToken);
                }
            }
        }

        public bool AutoRenewToken { get; set; } = true;

        public Credentials Credentials { get; set; }

        public HttpRequestHeaders Headers => HttpClient.DefaultRequestHeaders;

        public TimeSpan Timeout
        {
            get { return HttpClient.Timeout; }
            set { HttpClient.Timeout = value; }
        }

        public Task<RestResponse<string>> GetAsync(string resource) => GetAsync<string>(resource);

        public async Task<RestResponse<T>> GetAsync<T>(string resource)
        {
            using (var response = await InvokeResourceAsync<object>((arg, obj) => HttpClient.GetAsync(arg), resource, null))
            {
                var result = await GetContentAsync(response);
                return new RestResponse<T>(response, result);
            }
        }

        public Task<RestResponse> PostAsync(string resource) => PostAsync<object>(resource, null);

        public async Task<RestResponse> PostAsync<T>(string resource, T content)
        {
            using (var response = await InvokeResourceAsync((arg, obj) => HttpClient.PostAsJsonAsync(arg, obj), resource, content))
            {
                var result = await GetContentAsync(response);
                return new RestResponse(response, result);
            }
        }

        public Task<RestResponse<T>> PostWithResultAsync<T>(string resource) => PostWithResultAsync<object, T>(resource, null);

        public async Task<RestResponse<U>> PostWithResultAsync<T, U>(string resource, T content)
        {
            using (var response = await InvokeResourceAsync((arg, obj) => HttpClient.PostAsJsonAsync(arg, obj), resource, content))
            {
                var result = await GetContentAsync(response);
                return new RestResponse<U>(response, result);
            }
        }

        public Task<RestResponse> PutAsync(string resource) => PutAsync<object>(resource, null);

        public async Task<RestResponse> PutAsync<T>(string resource, T content)
        {
            using (var response = await InvokeResourceAsync((arg, obj) => HttpClient.PutAsJsonAsync(arg, obj), resource, content))
            {
                var result = await GetContentAsync(response);
                return new RestResponse(response, result);
            }
        }

        public Task<RestResponse<T>> PutWithResultAsync<T>(string resource) => PutWithResultAsync<object, T>(resource, null);

        public async Task<RestResponse<U>> PutWithResultAsync<T, U>(string resource, T content)
        {
            using (var response = await InvokeResourceAsync((arg, obj) => HttpClient.PutAsJsonAsync(arg, obj), resource, content))
            {
                var result = await GetContentAsync(response);
                return new RestResponse<U>(response, result);
            }
        }

        public async Task<RestResponse> DeleteAsync(string resource)
        {
            using (var response = await InvokeResourceAsync<object>((arg, obj) => HttpClient.DeleteAsync(arg), resource, null))
            {
                var result = await GetContentAsync(response);
                return new RestResponse(response, result);
            }
        }

        private async Task<HttpResponseMessage> InvokeResourceAsync<T>(Func<string, T, Task<HttpResponseMessage>> action, string resource, T obj)
        {
            resource = resource?.TrimStart('/');
            var response = await action(resource, obj).ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.Unauthorized && AutoRenewToken && Credentials != null)
            {
                await OAuthLoginAsync(Credentials.UserName, Credentials.Password).ConfigureAwait(false);
                response = await action.Invoke(resource, obj).ConfigureAwait(false);
            }

            return response;
        }

        private async Task<string> GetContentAsync(HttpResponseMessage response)
        {
            string content = null;

            try
            {
                content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            }
            catch
            {
            }

            return content;
        }

        public async Task<bool> OAuthLoginAsync(string userName, string password, string path = "token")
        {
            var body = $"grant_type=password&username={userName}&password={password}";
            var data = new StringContent(body, Encoding.UTF8, FormUrlEncoded);
            var response = await HttpClient.PostAsync(path, data).ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                // Invalid username or password.
                return false;
            }

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            AccessToken = JToken.Parse(content)["access_token"].ToString();
            Credentials = new Credentials(userName, password);

            return true;
        }

        public Task LogoutAsync()
        {
            AccessToken = null;
            Credentials = null;

            return Task.FromResult<object>(null);
        }

        public void Dispose()
        {
            HttpClient.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
