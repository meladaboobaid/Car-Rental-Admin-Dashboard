using Car_Rental_Dashboard.DTOs.Auth;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Security;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Car_Rental_Dashboard.APIs_Client
{
    public class UsersAPIs
    {
        public const string BaseUrl = "https://localhost:7035/";

        // Json options reused for streaming deserialization
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        // Create a HttpClient optimized for local dev & low latency
        public static HttpClient CreateHttpClientForLocalDev(string baseUrl)
        {
            var handler = new SocketsHttpHandler
            {
                // Keep connections healthy and rotate periodically
                PooledConnectionLifetime = TimeSpan.FromMinutes(5),
                PooledConnectionIdleTimeout = TimeSpan.FromMinutes(2),
                // Allow HTTP/2 when server supports it
                EnableMultipleHttp2Connections = true,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                UseCookies = false
            };

            // Accept self-signed certs in dev only
            handler.SslOptions = new System.Net.Security.SslClientAuthenticationOptions
            {
                // This disables validation for dev; remove in production.
                RemoteCertificateValidationCallback = (sender, cert, chain, sslPolicyErrors) =>
                    sslPolicyErrors == SslPolicyErrors.None || sslPolicyErrors == SslPolicyErrors.RemoteCertificateChainErrors
            };

            var client = new HttpClient(handler)
            {
                BaseAddress = new Uri(baseUrl),
                Timeout = Timeout.InfiniteTimeSpan // use per-call cancellation instead of global timeout
            };
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }

        // Login with streaming deserialization, cancellation and per-call timeout
        public static async Task<TokenResponse?> LoginAsync(HttpClient http, string email, string password, int timeoutMs = 10000, CancellationToken cancellationToken = default)
        {
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            linkedCts.CancelAfter(timeoutMs);

            var req = new LoginRequest { Email = email, Password = password };
            using var requestMsg = new HttpRequestMessage(HttpMethod.Post, "api/Auth/login")
            {
                Content = JsonContent.Create(req, options: _jsonOptions)
            };

            // Send only until headers are available so we can start processing earlier
            using var response = await http.SendAsync(requestMsg, HttpCompletionOption.ResponseHeadersRead, linkedCts.Token).ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return null;

            response.EnsureSuccessStatusCode();

            await using var stream = await response.Content.ReadAsStreamAsync(linkedCts.Token).ConfigureAwait(false);
            // Stream the small payload directly to the DTO
            var tokenResponse = await JsonSerializer.DeserializeAsync<TokenResponse>(stream, _jsonOptions, linkedCts.Token).ConfigureAwait(false);
            return tokenResponse;
        }

        public static async Task<HttpResponseMessage> SendWithAutoRefreshAsync(
            HttpClient http,
            HttpRequestMessage request,
            string email,
            TokenState tokenState)
        {
            // First attempt with current access token
            ApplyBearerToken(request, tokenState.AccessToken);
            var response = await http.SendAsync(request);

            // If not 401, return immediately
            if (response.StatusCode != HttpStatusCode.Unauthorized)
                return response;

            // 401 => access token expired/invalid => try refresh once
            response.Dispose();

            var newTokens = await RefreshTokensAsync(http, email, tokenState.RefreshToken);
            if (newTokens == null ||
                string.IsNullOrWhiteSpace(newTokens.AccessToken) ||
                string.IsNullOrWhiteSpace(newTokens.RefreshToken))
            {
                // Refresh failed => force re-login scenario
                return new HttpResponseMessage(HttpStatusCode.Unauthorized);
            }


            Global.CurrentUser.Token.RefreshToken = newTokens.RefreshToken;
            Global.CurrentUser.Token.AccessToken = newTokens.AccessToken;

            // Retry the original request once with new access token
            using var retryRequest = CloneRequest(request);
            ApplyBearerToken(retryRequest, tokenState.AccessToken);

            return await http.SendAsync(retryRequest);
        }

        static void ApplyBearerToken(HttpRequestMessage request, string accessToken)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        static async Task<TokenResponse?> RefreshTokensAsync(HttpClient http, string email, string refreshToken)
        {
            var request = new RefreshRequest
            {
                Email = email,
                RefreshToken = refreshToken
            };

            var response = await http.PostAsJsonAsync("/api/Auth/refresh", request);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<TokenResponse>();
        }


        // HttpRequestMessage cannot be re-sent once sent, so we clone it for retry.
        // For GET calls, this is enough. For POST/PUT with body, you'd need to recreate content too.
        static HttpRequestMessage CloneRequest(HttpRequestMessage original)
        {
            var clone = new HttpRequestMessage(original.Method, original.RequestUri);

            foreach (var header in original.Headers)
                clone.Headers.TryAddWithoutValidation(header.Key, header.Value);

            // Content is not used for GET here, but we copy if present.
            // Note: some HttpContent types are single-use; for robust POST/PUT retries,
            // recreate content from the original payload instead of copying.
            if (original.Content != null)
                clone.Content = original.Content;

            return clone;
        }
    }
}
