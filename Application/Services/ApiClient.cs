using Application.Abstractions;
using Application.Models.Logs;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Application.Services
{
    public class ApiClient : IApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ApiClient> _logger;
        private readonly JsonSerializerOptions _serializerOptions;

        public ApiClient(HttpClient httpClient, ILogger<ApiClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _serializerOptions = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
        }

        public async Task<HttpResponseMessage> GetAsync(string requestUrl)
        {
            try
            {
                var response = await _httpClient.GetAsync(requestUrl);

                LogResponse(response);
                response.EnsureSuccessStatusCode();
                // Normalize response
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception($"GET request failed: {ex.Message}", ex);
            }

        }

        public async Task<HttpResponseMessage> PostAsync<T>(string requestUrl, T payload)
        {
            try
            {
                var stringifiedPayload = JsonSerializer.Serialize(payload, _serializerOptions);
                var requestContent = new StringContent(stringifiedPayload, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(requestUrl, requestContent);

                LogResponse(response);
                response.EnsureSuccessStatusCode();
                // Normalize response
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception($"POST request failed: {ex.Message}", ex);
            }
        }

        public void SetBearerToken(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        private void LogResponse(HttpResponseMessage response, Exception? exception = null)
        {
            try
            {
                var state = new HttpResponseLog(response);
                var responseLog = JsonSerializer.Serialize(state, _serializerOptions);
                LogLevel logLvl = response.IsSuccessStatusCode ? LogLevel.Trace : LogLevel.Error;
                _logger.Log(logLvl, new EventId(0, "HttpResponseLog"), responseLog, exception, null);
            }
            catch { }
        }
    }
}
