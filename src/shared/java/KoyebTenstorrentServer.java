package com.tenstorrent.shared;

import com.sun.net.httpserver.Headers;
import com.sun.net.httpserver.HttpExchange;
import com.sun.net.httpserver.HttpHandler;
import com.sun.net.httpserver.HttpServer;
import java.io.IOException;
import java.io.OutputStream;
import java.net.InetSocketAddress;
import java.nio.charset.StandardCharsets;
import java.time.Instant;
import java.util.Arrays;
import java.util.regex.Matcher;
import java.util.regex.Pattern;
import java.util.logging.Level;
import java.util.logging.Logger;

public final class KoyebTenstorrentServer {
    private KoyebTenstorrentServer() {}

    private static final Logger LOGGER = Logging.getLogger(KoyebTenstorrentServer.class.getName());

    public static HttpServer start(int port) throws IOException {
        HttpServer server = HttpServer.create(new InetSocketAddress(port), 0);
        server.createContext(
                "/v1/chat/completions",
                new ChatCompletionsHandler());
        server.createContext(
                "/v1/completions",
                new CompletionsHandler());
        server.createContext(
                "/v1/embeddings",
                new EmbeddingsHandler());
        server.setExecutor(null);
        server.start();
        LOGGER.log(
                Level.INFO,
                () -> String.format(
                        "Tenstorrent stub server listening on port %d with level %s", port, LOGGER.getLevel()));
        return server;
    }

    public static void main(String[] args) throws IOException {
        int port = Integer.parseInt(System.getenv().getOrDefault("TT_SERVER_PORT", "8000"));
        start(port);
        System.out.println("Tenstorrent stub server listening on port " + port);
    }

    private static String readBody(HttpExchange exchange) throws IOException {
        return new String(exchange.getRequestBody().readAllBytes(), StandardCharsets.UTF_8);
    }

    private static void writeJson(HttpExchange exchange, String body) throws IOException {
        Headers headers = exchange.getResponseHeaders();
        headers.add("Content-Type", "application/json");
        byte[] payload = body.getBytes(StandardCharsets.UTF_8);
        exchange.sendResponseHeaders(200, payload.length);
        try (OutputStream os = exchange.getResponseBody()) {
            os.write(payload);
        }
    }

    private static String extractModel(String jsonBody, String fallback) {
        Pattern pattern = Pattern.compile("\"model\"\\s*:\\s*\"([^\"]+)\"");
        Matcher matcher = pattern.matcher(jsonBody);
        return matcher.find() ? matcher.group(1) : fallback;
    }

    private static final class ChatCompletionsHandler implements HttpHandler {
        @Override
        public void handle(HttpExchange exchange) throws IOException {
            String body = readBody(exchange);
            String model = extractModel(body, "local-tenstorrent");

            String message = TenstorrentLocalInference.generateChat(model, body);
            long created = Instant.now().getEpochSecond();

            LOGGER.fine(
                    () ->
                            String.format(
                                    "Handling chat completion for model %s (payload length %d)",
                                    model,
                                    body.length()));

            String response = String.format(
                    "{\"id\":\"chatcmpl-local-stub\",\"object\":\"chat.completion\","
                            + "\"created\":%d,\"model\":\"%s\",\"choices\":[{\"index\":0,"
                            + "\"message\":{\"role\":\"assistant\",\"content\":\"%s\"},"
                            + "\"finish_reason\":\"stop\"}],\"usage\":{\"prompt_tokens\":0,"
                            + "\"completion_tokens\":0,\"total_tokens\":0}}",
                    created,
                    model,
                    escape(message));

            writeJson(exchange, response);
        }
    }

    private static final class CompletionsHandler implements HttpHandler {
        @Override
        public void handle(HttpExchange exchange) throws IOException {
            String body = readBody(exchange);
            String model = extractModel(body, "local-tenstorrent");

            String completion = TenstorrentLocalInference.generateCompletion(model, body);
            long created = Instant.now().getEpochSecond();

            LOGGER.fine(
                    () ->
                            String.format(
                                    "Handling text completion for model %s (payload length %d)",
                                    model,
                                    body.length()));

            String response = String.format(
                    "{\"id\":\"cmpl-local-stub\",\"object\":\"text_completion\","
                            + "\"created\":%d,\"model\":\"%s\",\"choices\":[{\"index\":0,"
                            + "\"text\":\"%s\",\"finish_reason\":\"stop\"}],"
                            + "\"usage\":{\"prompt_tokens\":0,\"completion_tokens\":0,"
                            + "\"total_tokens\":0}}",
                    created,
                    model,
                    escape(completion));

            writeJson(exchange, response);
        }
    }

    private static final class EmbeddingsHandler implements HttpHandler {
        @Override
        public void handle(HttpExchange exchange) throws IOException {
            String body = readBody(exchange);
            String model = extractModel(body, "local-tenstorrent");

            double[] embedding = TenstorrentLocalInference.generateEmbedding(model, body);
            StringBuilder vectorBuilder = new StringBuilder("[");
            for (int i = 0; i < embedding.length; i++) {
                vectorBuilder.append(embedding[i]);
                if (i + 1 < embedding.length) {
                    vectorBuilder.append(',');
                }
            }
            vectorBuilder.append(']');

            LOGGER.fine(
                    () ->
                            String.format(
                                    "Handling embeddings for model %s (payload length %d)",
                                    model,
                                    body.length()));

            String response = String.format(
                    "{\"object\":\"list\",\"data\":[{\"object\":\"embedding\","
                            + "\"index\":0,\"embedding\":%s}],\"model\":\"%s\","
                            + "\"usage\":{\"prompt_tokens\":0,\"total_tokens\":0}}",
                    vectorBuilder,
                    model);

            writeJson(exchange, response);
        }
    }

    private static String escape(String text) {
        return text.replace("\\", "\\\\").replace("\"", "\\\"");
    }

    private static final class TenstorrentLocalInference {
        private TenstorrentLocalInference() {}

        static String generateChat(String model, String rawRequest) {
            return String.format(
                    "[stub] Replace with Tenstorrent chat inference for %s. Request=%s",
                    model,
                    truncate(rawRequest));
        }

        static String generateCompletion(String model, String rawRequest) {
            return String.format(
                    "[stub] Replace with Tenstorrent completion for %s. Request=%s",
                    model,
                    truncate(rawRequest));
        }

        static double[] generateEmbedding(String model, String rawRequest) {
            // Stub implementation ignores input; replace with Tenstorrent embeddings.
            double[] vector = new double[16];
            Arrays.fill(vector, 0.0);
            return vector;
        }

        private static String truncate(String text) {
            int max = 160;
            return text.length() <= max ? text : text.substring(0, max) + "...";
        }
    }
}
