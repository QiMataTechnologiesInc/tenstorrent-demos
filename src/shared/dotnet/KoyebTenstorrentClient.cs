using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Tenstorrent.Shared
{
    public sealed class KoyebTenstorrentClient : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly string _model;
        private readonly ILogger _logger;

        public KoyebTenstorrentClient(
            string baseUrl,
            string apiKey,
            string model,
            TimeSpan? timeout = null,
            HttpMessageHandler? handler = null,
            ILoggerFactory? loggerFactory = null)
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

            var factory = loggerFactory ?? Logging.CreateLoggerFactory();
            _logger = factory.CreateLogger<KoyebTenstorrentClient>();
            _logger.LogInformation(
                "Initialized KoyebTenstorrentClient for {BaseUrl} with model {Model}",
                normalizedBase,
                model);
        }

        public static KoyebTenstorrentClient FromEnvironment(
            string model,
            string baseUrlEnv = "KOYEB_TT_BASE_URL",
            string apiKeyEnv = "KOYEB_TT_API_KEY",
            string defaultApiKey = "fake",
            TimeSpan? timeout = null,
            ILoggerFactory? loggerFactory = null)
        {
            var baseUrl = Environment.GetEnvironmentVariable(baseUrlEnv);
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                throw new InvalidOperationException($"Environment variable {baseUrlEnv} is required for the base URL.");
            }

            var apiKey = Environment.GetEnvironmentVariable(apiKeyEnv) ?? defaultApiKey;
            return new KoyebTenstorrentClient(baseUrl, apiKey, model, timeout, loggerFactory: loggerFactory);
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

            _logger.LogDebug(
                "Sending chat completion request with {MessageCount} messages; stream={Stream}",
                (messages as ICollection<IDictionary<string, string>>)?.Count ?? 0,
                stream);

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

            _logger.LogDebug(
                "Sending text completion request (length={PromptLength}); stream={Stream}",
                prompt?.Length ?? 0,
                stream);

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

            _logger.LogDebug(
                "Sending embeddings request with {InputCount} inputs; dimensions={Dimensions}",
                (input as ICollection<string>)?.Count ?? 0,
                dimensions);

            return PostJsonAsync("embeddings", payload, cancellationToken);
        }

        private Task<HttpResponseMessage> PostJsonAsync(
            string relativePath,
            IReadOnlyDictionary<string, object?> payload,
            CancellationToken cancellationToken)
        {
            var content = new StringContent(JsonSerializer.Serialize(payload, JsonOptions), Encoding.UTF8, "application/json");
            _logger.LogInformation("POST {Path}", relativePath);
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
