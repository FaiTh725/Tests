using System.Net.Http.Headers;
using System.Net.Http.Json;
using Test.Integration.Tests.JwtToken;

namespace Test.Integration.Tests.Extensions
{
    public static class HttpClientExtension
    {
        public static Task<HttpResponseMessage> GetAsUserAsync(
            this HttpClient client,
            string requestUri,
            JwtUserData userData)
        {
            return client.AddBearerAuthHeader(JwtTokenProvider.CreateJwt(userData))
                .GetAsync(requestUri);
        }

        public static Task<HttpResponseMessage> PutAsUserAsync<TValue>(
            this HttpClient client,
            string requestUri,
            TValue value,
            JwtUserData userData)
        {
            return client.AddBearerAuthHeader(JwtTokenProvider.CreateJwt(userData))
                .PutAsJsonAsync(requestUri, value);
        }

        public static Task<HttpResponseMessage> PathAsUserAsync<TValue>(
            this HttpClient client,
            string requestUri,
            TValue value,
            JwtUserData userData)
        {
            return client.AddBearerAuthHeader(JwtTokenProvider.CreateJwt(userData))
                .PatchAsJsonAsync(requestUri, value);
        }

        public static Task<HttpResponseMessage> PostAsUserAsync<TValue>(
            this HttpClient client,
            string requestUri,
            TValue value,
            JwtUserData userData)
        {
            return client.AddBearerAuthHeader(JwtTokenProvider.CreateJwt(userData))
                .PostAsJsonAsync(requestUri, value);
        }

        public static Task<HttpResponseMessage> DeleteAsUserAsync(
            this HttpClient client,
            string requestUri,
            JwtUserData userData)
        {
            return client.AddBearerAuthHeader(JwtTokenProvider.CreateJwt(userData))
                .DeleteAsync(requestUri);
        }

        public static Task<HttpResponseMessage> DeleteAsUserAsync<TValue>(
            this HttpClient client,
            string requestUri,
            TValue value,
            JwtUserData userData)
        {
            var httpRequest = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                Content = JsonContent.Create(value),
                RequestUri = new Uri(requestUri, UriKind.Relative)
            };
            var userToken = JwtTokenProvider.CreateJwt(userData);
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userToken);
            httpRequest.Headers.Add("Cookie", "token=" + userToken);

            return client.SendAsync(httpRequest);
        }

        public static Task<HttpResponseMessage> PostFormData(
            this HttpClient client,
            string requestUri,
            MultipartFormDataContent content,
            JwtUserData userData)
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, requestUri);
            httpRequest.Content = content;
            var userToken = JwtTokenProvider.CreateJwt(userData);
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userToken);
            httpRequest.Headers.Add("Cookie", "token=" + userToken);

            return client.SendAsync(httpRequest);
        }

        public static Task<HttpResponseMessage> PatchFormData(
            this HttpClient client,
            string requestUri,
            MultipartFormDataContent content,
            JwtUserData userData)
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Patch, requestUri);
            httpRequest.Content = content;
            var userToken = JwtTokenProvider.CreateJwt(userData);
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userToken);
            httpRequest.Headers.Add("Cookie", "token=" + userToken);

            return client.SendAsync(httpRequest);
        }

        private static HttpClient AddBearerAuthHeader(
            this HttpClient client,
            string accessToken)
        {
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
            client.DefaultRequestHeaders.Add("Cookie", "token=" + accessToken);
            return client;
        }
    }
}
