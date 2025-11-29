using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Tenstorrent.Shared
{
    public static class KoyebTenstorrentServer
    {
        private static readonly string DefaultModel =
            Environment.GetEnvironmentVariable("TT_MODEL") ?? "local-tenstorrent";

        private static readonly IReadOnlyList<string> DefaultWormholeModels = new[]
        {
            "meta-llama/llama-3.1-8b-instruct",
            "meta-llama/llama-3.1-70b-instruct",
            "mistralai/mixtral-8x7b-instruct-v0.1",
        };

        private static readonly IReadOnlyList<string> WormholeSupportedModels = LoadWormholeSupportedModels();

        public static WebApplication Build(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var resolvedLevel = Logging.ResolveLogLevel();
            builder.Logging.ClearProviders();
            builder.Logging.SetMinimumLevel(resolvedLevel);
            builder.Logging.AddSimpleConsole(options =>
            {
                options.SingleLine = true;
                options.TimestampFormat = "yyyy-MM-dd HH:mm:ss ";
            });
            var app = builder.Build();
            var logger = app.Logger;

            logger.LogInformation(
                "Starting Tenstorrent stub server with log level {Level}",
                resolvedLevel);

            logger.LogInformation(
                "Default model {Model}; wormhole-supported models: {WormholeModels}",
                DefaultModel,
                string.Join(", ", WormholeSupportedModels));

            app.MapPost("/v1/chat/completions", async (
                ChatCompletionRequest request,
                CancellationToken cancellationToken) =>
            {
                var model = request.Model ?? DefaultModel;
                var cards = ResolveCardPlan(model, WormholeSupportedModels);
                var content = await TenstorrentLocalInference.GenerateChatAsync(
                    model,
                    request.Messages ?? new List<ChatMessage>(),
                    request.MaxTokens,
                    request.Temperature,
                    cards,
                    cancellationToken);

                logger.LogDebug(
                    "Handled chat completion for model {Model} with {MessageCount} messages on {Cards}",
                    model,
                    request.Messages?.Count ?? 0,
                    string.Join(", ", cards));

                var response = new ChatCompletionResponse
                {
                    Id = "chatcmpl-local-stub",
                    Model = model,
                    Created = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                    Choices = new List<ChatChoice>
                    {
                        new()
                        {
                            Index = 0,
                            Message = new ChatMessage { Role = "assistant", Content = content },
                            FinishReason = "stop",
                        },
                    },
                    Usage = TokenUsage.Zero,
                };

                return Results.Json(response);
            });

            app.MapPost("/v1/completions", async (
                CompletionRequest request,
                CancellationToken cancellationToken) =>
            {
                var model = request.Model ?? DefaultModel;
                var cards = ResolveCardPlan(model, WormholeSupportedModels);
                var text = await TenstorrentLocalInference.GenerateCompletionAsync(
                    model,
                    request.Prompt ?? string.Empty,
                    request.MaxTokens,
                    request.Temperature,
                    cards,
                    cancellationToken);

                logger.LogDebug(
                    "Handled text completion for model {Model} (prompt length {PromptLength}) on {Cards}",
                    model,
                    request.Prompt?.Length ?? 0,
                    string.Join(", ", cards));

                var response = new TextCompletionResponse
                {
                    Id = "cmpl-local-stub",
                    Model = model,
                    Created = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                    Choices = new List<TextChoice>
                    {
                        new()
                        {
                            Index = 0,
                            Text = text,
                            FinishReason = "stop",
                        },
                    },
                    Usage = TokenUsage.Zero,
                };

                return Results.Json(response);
            });

            app.MapPost("/v1/embeddings", async (
                EmbeddingsRequest request,
                CancellationToken cancellationToken) =>
            {
                var model = request.Model ?? DefaultModel;
                var cards = ResolveCardPlan(model, WormholeSupportedModels);
                var vector = await TenstorrentLocalInference.GenerateEmbeddingAsync(
                    model,
                    request.Input ?? Array.Empty<string>(),
                    request.Dimensions,
                    cards,
                    cancellationToken);

                logger.LogDebug(
                    "Handled embeddings for model {Model} with {InputCount} inputs on {Cards}",
                    model,
                    request.Input?.Length ?? 0,
                    string.Join(", ", cards));

                var response = new EmbeddingsResponse
                {
                    Object = "list",
                    Model = model,
                    Data = new List<EmbeddingResult>
                    {
                        new()
                        {
                            Object = "embedding",
                            Index = 0,
                            Embedding = vector,
                        },
                    },
                    Usage = new EmbeddingUsage { PromptTokens = 0, TotalTokens = 0 },
                };

                return Results.Json(response);
            });

            return app;
        }

        private static IReadOnlyList<string> LoadWormholeSupportedModels()
        {
            var configured = Environment.GetEnvironmentVariable("TT_WORMHOLE_SUPPORTED_MODELS");
            if (string.IsNullOrWhiteSpace(configured))
            {
                return DefaultWormholeModels;
            }

            var parsed = configured
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(entry => entry.Trim())
                .Where(entry => !string.IsNullOrEmpty(entry))
                .Select(entry => entry.ToLowerInvariant())
                .ToArray();

            return parsed.Length == 0 ? DefaultWormholeModels : parsed;
        }

        private static IReadOnlyList<string> ResolveCardPlan(
            string model, IReadOnlyList<string> wormholeSupportedModels)
        {
            var normalized = (model ?? string.Empty).Trim().ToLowerInvariant();
            foreach (var supported in wormholeSupportedModels)
            {
                if (normalized == supported || normalized.StartsWith(supported, StringComparison.Ordinal))
                {
                    return new[] { "wormhole", "blackhole" };
                }
            }

            return new[] { "blackhole" };
        }
    }

    public static class TenstorrentLocalInference
    {
        public static Task<string> GenerateChatAsync(
            string model,
            IReadOnlyList<ChatMessage> messages,
            int? maxTokens,
            double? temperature,
            IReadOnlyList<string> cards,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _ = messages;
            _ = maxTokens;
            _ = temperature;
            var cardPlan = string.Join(" + ", cards ?? Array.Empty<string>());
            return Task.FromResult($"[stub] Run Tenstorrent chat for model {model} using {cardPlan}.");
        }

        public static Task<string> GenerateCompletionAsync(
            string model,
            string prompt,
            int? maxTokens,
            double? temperature,
            IReadOnlyList<string> cards,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _ = prompt;
            _ = maxTokens;
            _ = temperature;
            var cardPlan = string.Join(" + ", cards ?? Array.Empty<string>());
            return Task.FromResult($"[stub] Run Tenstorrent completion for model {model} using {cardPlan}.");
        }

        public static Task<IReadOnlyList<double>> GenerateEmbeddingAsync(
            string model,
            IReadOnlyList<string> input,
            int? dimensions,
            IReadOnlyList<string> cards,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _ = model;
            _ = input;
            _ = cards;

            var dim = dimensions ?? 16;
            var values = new double[dim];
            return Task.FromResult((IReadOnlyList<double>)values);
        }
    }

    public sealed class ChatCompletionRequest
    {
        [JsonPropertyName("model")]
        public string? Model { get; set; }

        [JsonPropertyName("messages")]
        public List<ChatMessage>? Messages { get; set; }

        [JsonPropertyName("max_tokens")]
        public int? MaxTokens { get; set; }

        [JsonPropertyName("temperature")]
        public double? Temperature { get; set; }

        [JsonPropertyName("stream")]
        public bool Stream { get; set; }
    }

    public sealed class CompletionRequest
    {
        [JsonPropertyName("model")]
        public string? Model { get; set; }

        [JsonPropertyName("prompt")]
        public string? Prompt { get; set; }

        [JsonPropertyName("max_tokens")]
        public int? MaxTokens { get; set; }

        [JsonPropertyName("temperature")]
        public double? Temperature { get; set; }

        [JsonPropertyName("stream")]
        public bool Stream { get; set; }
    }

    public sealed class EmbeddingsRequest
    {
        [JsonPropertyName("model")]
        public string? Model { get; set; }

        [JsonPropertyName("input")]
        public string[]? Input { get; set; }

        [JsonPropertyName("dimensions")]
        public int? Dimensions { get; set; }
    }

    public sealed class ChatMessage
    {
        [JsonPropertyName("role")]
        public string Role { get; set; } = "assistant";

        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;
    }

    public sealed class ChatCompletionResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("object")]
        public string Object { get; set; } = "chat.completion";

        [JsonPropertyName("created")]
        public long Created { get; set; }

        [JsonPropertyName("model")]
        public string Model { get; set; } = string.Empty;

        [JsonPropertyName("choices")]
        public List<ChatChoice> Choices { get; set; } = new();

        [JsonPropertyName("usage")]
        public TokenUsage Usage { get; set; } = TokenUsage.Zero;
    }

    public sealed class ChatChoice
    {
        [JsonPropertyName("index")]
        public int Index { get; set; }

        [JsonPropertyName("message")]
        public ChatMessage Message { get; set; } = new();

        [JsonPropertyName("finish_reason")]
        public string FinishReason { get; set; } = "stop";
    }

    public sealed class TextCompletionResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("object")]
        public string Object { get; set; } = "text_completion";

        [JsonPropertyName("created")]
        public long Created { get; set; }

        [JsonPropertyName("model")]
        public string Model { get; set; } = string.Empty;

        [JsonPropertyName("choices")]
        public List<TextChoice> Choices { get; set; } = new();

        [JsonPropertyName("usage")]
        public TokenUsage Usage { get; set; } = TokenUsage.Zero;
    }

    public sealed class TextChoice
    {
        [JsonPropertyName("index")]
        public int Index { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty;

        [JsonPropertyName("finish_reason")]
        public string FinishReason { get; set; } = "stop";
    }

    public sealed class EmbeddingsResponse
    {
        [JsonPropertyName("object")]
        public string Object { get; set; } = "list";

        [JsonPropertyName("data")]
        public List<EmbeddingResult> Data { get; set; } = new();

        [JsonPropertyName("model")]
        public string Model { get; set; } = string.Empty;

        [JsonPropertyName("usage")]
        public EmbeddingUsage Usage { get; set; } = new();
    }

    public sealed class EmbeddingResult
    {
        [JsonPropertyName("object")]
        public string Object { get; set; } = "embedding";

        [JsonPropertyName("embedding")]
        public IReadOnlyList<double> Embedding { get; set; } = Array.Empty<double>();

        [JsonPropertyName("index")]
        public int Index { get; set; }
    }

    public sealed class TokenUsage
    {
        [JsonPropertyName("prompt_tokens")]
        public int PromptTokens { get; set; }

        [JsonPropertyName("completion_tokens")]
        public int CompletionTokens { get; set; }

        [JsonPropertyName("total_tokens")]
        public int TotalTokens { get; set; }

        public static TokenUsage Zero => new() { PromptTokens = 0, CompletionTokens = 0, TotalTokens = 0 };
    }

    public sealed class EmbeddingUsage
    {
        [JsonPropertyName("prompt_tokens")]
        public int PromptTokens { get; set; }

        [JsonPropertyName("total_tokens")]
        public int TotalTokens { get; set; }
    }
}
