package com.tenstorrent.shared;

import java.net.URI;
import java.net.http.HttpClient;
import java.net.http.HttpRequest;
import java.net.http.HttpResponse;
import java.time.Duration;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.Objects;

public class KoyebTenstorrentClient {
    private final HttpClient httpClient;
    private final URI baseUri;
    private final String model;
    private final String apiKey;

    public static class ChatMessage {
        public final String role;
        public final String content;

        public ChatMessage(String role, String content) {
            this.role = Objects.requireNonNull(role, "role");
            this.content = Objects.requireNonNull(content, "content");
        }
    }

    public KoyebTenstorrentClient(String baseUrl, String apiKey, String model, Duration timeout) {
        if (baseUrl == null || baseUrl.isBlank()) {
            throw new IllegalArgumentException("Base URL is required");
        }
        this.apiKey = Objects.requireNonNull(apiKey, "apiKey");
        this.model = Objects.requireNonNull(model, "model");

        String normalized = baseUrl.endsWith("/") ? baseUrl.substring(0, baseUrl.length() - 1) : baseUrl;
        this.baseUri = URI.create(normalized + "/v1/");

        HttpClient.Builder builder = HttpClient.newBuilder();
        if (timeout != null) {
            builder.connectTimeout(timeout);
        }
        this.httpClient = builder.build();
    }

    public static KoyebTenstorrentClient fromEnvironment(String model) {
        return fromEnvironment(model, "KOYEB_TT_BASE_URL", "KOYEB_TT_API_KEY", "fake", null);
    }

    public static KoyebTenstorrentClient fromEnvironment(
            String model, String baseUrlEnv, String apiKeyEnv, String defaultApiKey, Duration timeout) {
        String baseUrl = System.getenv(baseUrlEnv);
        if (baseUrl == null || baseUrl.isBlank()) {
            throw new IllegalStateException("Environment variable " + baseUrlEnv + " is required for the base URL.");
        }
        String apiKey = System.getenv(apiKeyEnv);
        if (apiKey == null || apiKey.isBlank()) {
            apiKey = defaultApiKey;
        }
        return new KoyebTenstorrentClient(baseUrl, apiKey, model, timeout);
    }

    public HttpResponse<String> createChatCompletion(
            List<ChatMessage> messages, Integer maxTokens, Double temperature, boolean stream) throws Exception {
        Map<String, Object> payload = new HashMap<>();
        payload.put("model", model);
        payload.put("stream", stream);
        payload.put("messages", messages.stream()
                .map(m -> Map.of("role", m.role, "content", m.content))
                .toList());
        if (maxTokens != null) {
            payload.put("max_tokens", maxTokens);
        }
        if (temperature != null) {
            payload.put("temperature", temperature);
        }
        return post("chat/completions", payload);
    }

    public HttpResponse<String> createCompletion(String prompt, Integer maxTokens, Double temperature, boolean stream) throws Exception {
        Map<String, Object> payload = new HashMap<>();
        payload.put("model", model);
        payload.put("prompt", prompt);
        payload.put("stream", stream);
        if (maxTokens != null) {
            payload.put("max_tokens", maxTokens);
        }
        if (temperature != null) {
            payload.put("temperature", temperature);
        }
        return post("completions", payload);
    }

    public HttpResponse<String> createEmbeddings(List<String> input, Integer dimensions) throws Exception {
        Map<String, Object> payload = new HashMap<>();
        payload.put("model", model);
        payload.put("input", input);
        if (dimensions != null) {
            payload.put("dimensions", dimensions);
        }
        return post("embeddings", payload);
    }

    private HttpResponse<String> post(String path, Map<String, Object> payload) throws Exception {
        HttpRequest request = HttpRequest.newBuilder()
                .uri(baseUri.resolve(path))
                .header("Content-Type", "application/json")
                .header("Authorization", "Bearer " + apiKey)
                .POST(HttpRequest.BodyPublishers.ofString(serialize(payload)))
                .build();

        return httpClient.send(request, HttpResponse.BodyHandlers.ofString());
    }

    private static String serialize(Object value) {
        if (value == null) {
            return "null";
        }
        if (value instanceof String s) {
            return '"' + escape(s) + '"';
        }
        if (value instanceof Number || value instanceof Boolean) {
            return value.toString();
        }
        if (value instanceof Map<?, ?> map) {
            StringBuilder builder = new StringBuilder();
            builder.append('{');
            boolean first = true;
            for (Map.Entry<?, ?> entry : map.entrySet()) {
                if (!first) {
                    builder.append(',');
                }
                builder.append('"').append(escape(entry.getKey().toString())).append('"').append(':');
                builder.append(serialize(entry.getValue()));
                first = false;
            }
            builder.append('}');
            return builder.toString();
        }
        if (value instanceof Iterable<?> iterable) {
            StringBuilder builder = new StringBuilder();
            builder.append('[');
            boolean first = true;
            for (Object item : iterable) {
                if (!first) {
                    builder.append(',');
                }
                builder.append(serialize(item));
                first = false;
            }
            builder.append(']');
            return builder.toString();
        }
        return '"' + escape(value.toString()) + '"';
    }

    private static String escape(String value) {
        return value
                .replace("\\", "\\\\")
                .replace("\"", "\\\"")
                .replace("\n", "\\n");
    }
}
