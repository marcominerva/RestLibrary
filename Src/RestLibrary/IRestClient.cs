using RestLibrary.Models;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace RestLibrary
{
    public interface IRestClient : IDisposable
    {
        string BaseAddress { get; set; }

        string Language { get; set; }

        string AccessToken { get; set; }

        bool AutoRenewToken { get; set; }

        Credentials Credentials { get; set; }

        HttpRequestHeaders Headers { get; }

        TimeSpan Timeout { get; set; }

        HttpClient HttpClient { get; }

        Task<RestResponse<string>> GetAsync(string resource);

        Task<RestResponse<T>> GetAsync<T>(string resource);

        Task<RestResponse> PostAsync(string resource);

        Task<RestResponse> PostAsync<T>(string resource, T content);

        Task<RestResponse<T>> PostWithResultAsync<T>(string resource);

        Task<RestResponse<U>> PostWithResultAsync<T, U>(string resource, T content);

        Task<RestResponse> PutAsync(string resource);

        Task<RestResponse> PutAsync<T>(string resource, T content);

        Task<RestResponse<T>> PutWithResultAsync<T>(string resource);

        Task<RestResponse<U>> PutWithResultAsync<T, U>(string resource, T content);

        Task<RestResponse> DeleteAsync(string resource);

        Task<bool> OAuthLoginAsync(string userName, string password, string path = "token");

        Task LogoutAsync();
    }
}