using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Tenstorrent.Shared
{
    public sealed class KoyebTenstorrentClient : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly string _model;

        public KoyebTenstorrentClient(
            string baseUrl,
            string apiKey,
            string model,
            TimeSpan? timeout = null,
            HttpMessageHandler? handler = null)
        {
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                throw new ArgumentException("Base URL is required", nameof(baseUrl));
            }

            _model = model ?? throw new ArgumentNullException(nameof(model));

            var normalizedBase = baseUrl.TrimEnd('/') + "/v1/";
            _httpClient = handler == null ? new HttpClient() : new HttpClient(handler, disposeHandler: false);
            _httpClient.BaseAddress = new Uri(normalizedBase);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            if (timeout.HasValue)
            {
                _httpClient.Timeout = timeout.Value;
            }
        }

        public static KoyebTenstorrentClient FromEnvironment(
            string model,
            string baseUrlEnv = "KOYEB_TT_BASE_URL",
            string apiKeyEnv = "KOYEB_TT_API_KEY",
            string defaultApiKey = "fake",
            TimeSpan? timeout = null)
        {
            var baseUrl = Environment.GetEnvironmentVariable(baseUrlEnv);
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                throw new InvalidOperationException($"Environment variable {baseUrlEnv} is required for the base URL.");
            }

            var apiKey = Environment.GetEnvironmentVariable(apiKeyEnv) ?? defaultApiKey;
            return new KoyebTenstorrentClient(baseUrl, apiKey, model, timeout);
        }

        public Task<HttpResponseMessage> CreateChatCompletionAsync(
            IEnumerable<IDictionary<string, string>> messages,
            int? maxTokens = null,
            double? temperature = null,
            bool stream = false,
            CancellationToken cancellationToken = default)
        {
            var payload = new Dictionary<string, object?>
            {
                ["model"] = _model,
                ["messages"] = messages,
                ["max_tokens"] = maxTokens,
                ["temperature"] = temperature,
                ["stream"] = stream,
            };

            return PostJsonAsync("chat/completions", payload, cancellationToken);
        }

        public Task<HttpResponseMessage> CreateCompletionAsync(
            string prompt,
            int? maxTokens = null,
            double? temperature = null,
            bool stream = false,
            CancellationToken cancellationToken = default)
        {
            var payload = new Dictionary<string, object?>
            {
                ["model"] = _model,
                ["prompt"] = prompt,
                ["max_tokens"] = maxTokens,
                ["temperature"] = temperature,
                ["stream"] = stream,
            };

            return PostJsonAsync("completions", payload, cancellationToken);
        }

        public Task<HttpResponseMessage> CreateEmbeddingsAsync(
            IEnumerable<string> input,
            int? dimensions = null,
            CancellationToken cancellationToken = default)
        {
            var payload = new Dictionary<string, object?>
            {
                ["model"] = _model,
                ["input"] = input,
                ["dimensions"] = dimensions,
            };

            return PostJsonAsync("embeddings", payload, cancellationToken);
        }

        private Task<HttpResponseMessage> PostJsonAsync(
            string relativePath,
            IReadOnlyDictionary<string, object?> payload,
            CancellationToken cancellationToken)
        {
            var content = new StringContent(JsonSerializer.Serialize(payload, JsonOptions), Encoding.UTF8, "application/json");
            return _httpClient.PostAsync(relativePath, content, cancellationToken);
        }

        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
        };

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
