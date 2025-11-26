#include "crow.h"

#include <chrono>
#include <string>
#include <vector>

// Stub helpers for connecting to local Tenstorrent hardware. Replace these with
// calls into TT-NN / TT-Metalium or other bindings when available.
std::string run_tenstorrent_chat(
    const crow::json::rvalue& messages,
    const std::string& model,
    int /*max_tokens*/,
    double /*temperature*/)
{
    (void)messages;
    return "[stub] Integrate Tenstorrent chat inference for model " + model;
}

std::string run_tenstorrent_completion(
    const std::string& prompt,
    const std::string& model,
    int /*max_tokens*/,
    double /*temperature*/)
{
    (void)prompt;
    return "[stub] Integrate Tenstorrent completion for model " + model;
}

std::vector<double> run_tenstorrent_embedding(
    const crow::json::rvalue& input,
    const std::string& /*model*/,
    int dimensions)
{
    (void)input;
    if (dimensions <= 0)
    {
        dimensions = 16;
    }
    return std::vector<double>(static_cast<std::size_t>(dimensions), 0.0);
}

int main()
{
    crow::SimpleApp app;

    CROW_ROUTE(app, "/v1/chat/completions").methods("POST"_method)([] (const crow::request& req) {
        auto body = crow::json::load(req.body);
        if (!body)
        {
            return crow::response(400, "Invalid JSON payload");
        }

        auto model = body["model"].s("local-tenstorrent");
        auto messages = body["messages"];
        int max_tokens = body["max_tokens"].i();
        double temperature = body["temperature"].d();
        auto content = run_tenstorrent_chat(messages, model, max_tokens, temperature);

        crow::json::wvalue response;
        response["id"] = "chatcmpl-local-stub";
        response["object"] = "chat.completion";
        response["created"] = static_cast<std::int64_t>(
            std::chrono::duration_cast<std::chrono::seconds>(
                std::chrono::system_clock::now().time_since_epoch())
                .count());
        response["model"] = model;

        crow::json::wvalue choice;
        choice["index"] = 0;
        choice["message"]["role"] = "assistant";
        choice["message"]["content"] = content;
        choice["finish_reason"] = "stop";
        response["choices"] = crow::json::wvalue::list({choice});

        response["usage"]["prompt_tokens"] = 0;
        response["usage"]["completion_tokens"] = 0;
        response["usage"]["total_tokens"] = 0;

        crow::response res;
        res.code = 200;
        res.set_header("Content-Type", "application/json");
        res.body = crow::json::dump(response);
        return res;
    });

    CROW_ROUTE(app, "/v1/completions").methods("POST"_method)([] (const crow::request& req) {
        auto body = crow::json::load(req.body);
        if (!body)
        {
            return crow::response(400, "Invalid JSON payload");
        }

        auto model = body["model"].s("local-tenstorrent");
        auto prompt = body["prompt"].s("");
        int max_tokens = body["max_tokens"].i();
        double temperature = body["temperature"].d();
        auto text = run_tenstorrent_completion(prompt, model, max_tokens, temperature);

        crow::json::wvalue response;
        response["id"] = "cmpl-local-stub";
        response["object"] = "text_completion";
        response["created"] = static_cast<std::int64_t>(
            std::chrono::duration_cast<std::chrono::seconds>(
                std::chrono::system_clock::now().time_since_epoch())
                .count());
        response["model"] = model;

        crow::json::wvalue choice;
        choice["index"] = 0;
        choice["text"] = text;
        choice["logprobs"] = nullptr;
        choice["finish_reason"] = "stop";
        response["choices"] = crow::json::wvalue::list({choice});

        response["usage"]["prompt_tokens"] = 0;
        response["usage"]["completion_tokens"] = 0;
        response["usage"]["total_tokens"] = 0;

        crow::response res;
        res.code = 200;
        res.set_header("Content-Type", "application/json");
        res.body = crow::json::dump(response);
        return res;
    });

    CROW_ROUTE(app, "/v1/embeddings").methods("POST"_method)([] (const crow::request& req) {
        auto body = crow::json::load(req.body);
        if (!body)
        {
            return crow::response(400, "Invalid JSON payload");
        }

        auto model = body["model"].s("local-tenstorrent");
        int dimensions = body["dimensions"].i();
        auto vector = run_tenstorrent_embedding(body["input"], model, dimensions);

        crow::json::wvalue data;
        data["object"] = "embedding";
        data["index"] = 0;
        data["embedding"] = crow::json::wvalue::list(vector);

        crow::json::wvalue response;
        response["object"] = "list";
        response["data"] = crow::json::wvalue::list({data});
        response["model"] = model;
        response["usage"]["prompt_tokens"] = 0;
        response["usage"]["total_tokens"] = 0;

        crow::response res;
        res.code = 200;
        res.set_header("Content-Type", "application/json");
        res.body = crow::json::dump(response);
        return res;
    });

    unsigned short port = 8000;
    app.port(port).multithreaded().run();
    return 0;
}
