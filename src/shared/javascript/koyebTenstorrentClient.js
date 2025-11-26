import { createLogger } from "./logging.js";

const assertNonEmpty = (value, name) => {
  if (!value || String(value).trim() === "") {
    throw new Error(`${name} is required`);
  }
};

export class KoyebTenstorrentClient {
  constructor(baseUrl, apiKey, model, { timeoutMs } = {}) {
    assertNonEmpty(baseUrl, "Base URL");
    assertNonEmpty(model, "Model");

    this.baseUrl = baseUrl.replace(/\/$/, "");
    this.apiKey = apiKey ?? "fake";
    this.model = model;
    this.timeoutMs = timeoutMs;
    this.logger = createLogger("KoyebTTClient");

    this.logger.info("Initialized KoyebTenstorrentClient", {
      baseUrl: this.baseUrl,
      model: this.model,
      timeoutMs: this.timeoutMs,
      logLevel: this.logger.level,
    });
  }

  static fromEnvironment(model, { baseUrlEnv = "KOYEB_TT_BASE_URL", apiKeyEnv = "KOYEB_TT_API_KEY", defaultApiKey = "fake", timeoutMs } = {}) {
    const baseUrl = process.env[baseUrlEnv];
    if (!baseUrl) {
      throw new Error(`Environment variable ${baseUrlEnv} is required for the base URL.`);
    }
    const apiKey = process.env[apiKeyEnv] ?? defaultApiKey;
    return new KoyebTenstorrentClient(baseUrl, apiKey, model, { timeoutMs });
  }

  async createChatCompletion(messages, { max_tokens, temperature, stream = false } = {}) {
    this.logger.debug("Sending chat completion request", {
      messageCount: messages?.length ?? 0,
      max_tokens,
      temperature,
      stream,
    });
    return this.#post("/chat/completions", {
      model: this.model,
      messages,
      max_tokens,
      temperature,
      stream,
    });
  }

  async createCompletion(prompt, { max_tokens, temperature, stream = false } = {}) {
    this.logger.debug("Sending text completion request", {
      promptLength: prompt?.length ?? 0,
      max_tokens,
      temperature,
      stream,
    });
    return this.#post("/completions", {
      model: this.model,
      prompt,
      max_tokens,
      temperature,
      stream,
    });
  }

  async createEmbeddings(input, { dimensions } = {}) {
    this.logger.debug("Sending embedding request", {
      inputCount: input?.length ?? 0,
      dimensions,
    });
    return this.#post("/embeddings", {
      model: this.model,
      input,
      dimensions,
    });
  }

  async #post(path, payload) {
    const controller = this.timeoutMs ? new AbortController() : null;
    const url = `${this.baseUrl}/v1${path}`;
    const timeoutId = this.timeoutMs ? setTimeout(() => controller.abort(), this.timeoutMs) : null;

    try {
      this.logger.info("POSTing Tenstorrent request", {
        url,
        path,
        timeoutMs: this.timeoutMs,
      });
      const response = await fetch(url, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${this.apiKey}`,
        },
        body: JSON.stringify(payload),
        signal: controller?.signal,
      });

      return response;
    } finally {
      if (timeoutId) {
        clearTimeout(timeoutId);
      }
    }
  }
}
