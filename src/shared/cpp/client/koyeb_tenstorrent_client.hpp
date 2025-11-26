#pragma once

#include <cpr/cpr.h>
#include <nlohmann/json.hpp>

#include <chrono>
#include <optional>
#include <stdexcept>
#include <string>
#include <utility>
#include <vector>

namespace tenstorrent::shared {

struct ChatMessage {
    std::string role;
    std::string content;
};

class KoyebTenstorrentClient {
public:
    KoyebTenstorrentClient(std::string base_url, std::string api_key, std::string model,
                           std::optional<std::chrono::milliseconds> timeout = std::nullopt)
        : base_url_(std::move(base_url)), api_key_(std::move(api_key)), model_(std::move(model)),
          timeout_(timeout) {
        if (base_url_.empty()) {
            throw std::invalid_argument("Base URL is required");
        }
        if (model_.empty()) {
            throw std::invalid_argument("Model name is required");
        }

        if (base_url_.back() == '/') {
            base_url_.pop_back();
        }
        base_url_ += "/v1";
    }

    static KoyebTenstorrentClient from_environment(
        const std::string& model,
        const std::string& base_url_env = "KOYEB_TT_BASE_URL",
        const std::string& api_key_env = "KOYEB_TT_API_KEY",
        const std::string& default_api_key = "fake",
        std::optional<std::chrono::milliseconds> timeout = std::nullopt) {
        const char* base = std::getenv(base_url_env.c_str());
        if (base == nullptr || std::string(base).empty()) {
            throw std::runtime_error("Environment variable " + base_url_env + " is required for the base URL.");
        }

        const char* key = std::getenv(api_key_env.c_str());
        return KoyebTenstorrentClient(base, key == nullptr ? default_api_key : key, model, timeout);
    }

    cpr::Response create_chat_completion(const std::vector<ChatMessage>& messages, std::optional<int> max_tokens = std::nullopt,
                                         std::optional<double> temperature = std::nullopt, bool stream = false) const {
        nlohmann::json payload{{"model", model_}, {"stream", stream}};
        payload["messages"] = nlohmann::json::array();
        for (const auto& message : messages) {
            payload["messages"].push_back({{"role", message.role}, {"content", message.content}});
        }

        if (max_tokens.has_value()) {
            payload["max_tokens"] = max_tokens.value();
        }
        if (temperature.has_value()) {
            payload["temperature"] = temperature.value();
        }

        return post("/chat/completions", payload.dump());
    }

    cpr::Response create_completion(const std::string& prompt, std::optional<int> max_tokens = std::nullopt,
                                    std::optional<double> temperature = std::nullopt, bool stream = false) const {
        nlohmann::json payload{{"model", model_}, {"prompt", prompt}, {"stream", stream}};
        if (max_tokens.has_value()) {
            payload["max_tokens"] = max_tokens.value();
        }
        if (temperature.has_value()) {
            payload["temperature"] = temperature.value();
        }

        return post("/completions", payload.dump());
    }

    cpr::Response create_embeddings(const std::vector<std::string>& input, std::optional<int> dimensions = std::nullopt) const {
        nlohmann::json payload{{"model", model_}, {"input", input}};
        if (dimensions.has_value()) {
            payload["dimensions"] = dimensions.value();
        }
        return post("/embeddings", payload.dump());
    }

private:
    cpr::Response post(const std::string& route, const std::string& json_body) const {
        cpr::Header headers{{"Authorization", "Bearer " + api_key_}, {"Content-Type", "application/json"}};

        cpr::Session session;
        session.SetUrl(cpr::Url{base_url_ + route});
        session.SetHeader(headers);
        session.SetBody(cpr::Body{json_body});
        if (timeout_.has_value()) {
            session.SetTimeout(cpr::Timeout{timeout_.value()});
        }
        return session.Post();
    }

    mutable std::string base_url_;
    std::string api_key_;
    std::string model_;
    std::optional<std::chrono::milliseconds> timeout_;
};

}  // namespace tenstorrent::shared
