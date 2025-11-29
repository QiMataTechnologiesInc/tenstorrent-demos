import { createServer } from "http";
import { createLogger } from "./logging.js";

const DEFAULT_WORMHOLE_MODELS = new Set([
  "meta-llama/llama-3.1-8b-instruct",
  "meta-llama/llama-3.1-70b-instruct",
  "mistralai/mixtral-8x7b-instruct-v0.1",
]);

const normalizeModel = (model) => (model ?? "").trim().toLowerCase();

const loadWormholeSupportedModels = (
  envVar = "TT_WORMHOLE_SUPPORTED_MODELS"
) => {
  const configured = process.env[envVar];
  if (!configured) {
    return new Set(DEFAULT_WORMHOLE_MODELS);
  }

  const entries = configured
    .split(",")
    .map((entry) => entry.trim())
    .filter((entry) => entry.length > 0)
    .map((entry) => entry.toLowerCase());

  return new Set(entries);
};

const resolveCardPlan = (model, wormholeSupportedModels) => {
  const normalized = normalizeModel(model);
  for (const supported of wormholeSupportedModels) {
    if (normalized === supported || normalized.startsWith(supported)) {
      return ["wormhole", "blackhole"];
    }
  }
  return ["blackhole"];
};

const readBody = async (req) => {
  const chunks = [];
  for await (const chunk of req) {
    chunks.push(chunk);
  }
  return Buffer.concat(chunks).toString("utf8");
};

const sendJson = (res, payload) => {
  const body = JSON.stringify(payload);
  res.writeHead(200, { "Content-Type": "application/json" });
  res.end(body);
};

const runTenstorrentChat = (
  messages,
  model,
  cards,
  { max_tokens, temperature }
) => {
  return {
    id: "chatcmpl-local-stub",
    object: "chat.completion",
    created: Math.floor(Date.now() / 1000),
    model,
    choices: [
      {
        index: 0,
        message: {
          role: "assistant",
          content:
            `[stub] Replace with Tenstorrent chat inference for ${model} using ${cards.join(
              " + "
            )}.`,
        },
        finish_reason: "stop",
      },
    ],
    usage: { prompt_tokens: 0, completion_tokens: 0, total_tokens: 0 },
  };
};

const runTenstorrentCompletion = (
  prompt,
  model,
  cards,
  { max_tokens, temperature }
) => {
  return {
    id: "cmpl-local-stub",
    object: "text_completion",
    created: Math.floor(Date.now() / 1000),
    model,
    choices: [
      {
        index: 0,
        text: `[stub] Replace with Tenstorrent completion for ${model} using ${cards.join(
          " + "
        )}.`,
        finish_reason: "stop",
      },
    ],
    usage: { prompt_tokens: 0, completion_tokens: 0, total_tokens: 0 },
  };
};

const runTenstorrentEmbedding = (input, model, cards, { dimensions }) => {
  const dim = dimensions ?? 16;
  return {
    object: "list",
    data: [
      {
        object: "embedding",
        index: 0,
        embedding: Array(dim).fill(0),
      },
    ],
    model,
    usage: { prompt_tokens: 0, total_tokens: 0 },
  };
};

export const startTenstorrentServer = ({
  port = Number(process.env.TT_SERVER_PORT ?? 8000),
  defaultModel = process.env.TT_MODEL ?? "local-tenstorrent",
  wormholeSupportedModels = loadWormholeSupportedModels(),
} = {}) => {
  const logger = createLogger("KoyebTTServer");

  logger.info("Starting Tenstorrent stub server", {
    port,
    defaultModel,
    wormholeSupportedModels: Array.from(wormholeSupportedModels),
    logLevel: logger.level,
  });

  const server = createServer(async (req, res) => {
    const url = new URL(req.url, "http://localhost");

    if (req.method !== "POST") {
      res.statusCode = 404;
      res.end();
      return;
    }

    const bodyText = await readBody(req);
    const payload = bodyText ? JSON.parse(bodyText) : {};
    const model = payload.model ?? defaultModel;
    const cards = resolveCardPlan(model, wormholeSupportedModels);

    if (url.pathname === "/v1/chat/completions") {
      logger.debug("Handling chat completion", {
        model,
        messageCount: payload.messages?.length ?? 0,
        cards,
      });
      sendJson(
        res,
        runTenstorrentChat(payload.messages ?? [], model, cards, payload)
      );
      return;
    }

    if (url.pathname === "/v1/completions") {
      logger.debug("Handling text completion", {
        model,
        promptLength: payload.prompt?.length ?? 0,
        cards,
      });
      sendJson(
        res,
        runTenstorrentCompletion(payload.prompt ?? "", model, cards, payload)
      );
      return;
    }

    if (url.pathname === "/v1/embeddings") {
      logger.debug("Handling embeddings", {
        model,
        inputCount: payload.input?.length ?? 0,
        cards,
      });
      sendJson(
        res,
        runTenstorrentEmbedding(payload.input ?? [], model, cards, payload)
      );
      return;
    }

    res.statusCode = 404;
    res.end();
  });

  server.listen(port, "0.0.0.0", () => {
    logger.info("Tenstorrent server listening", { port });
  });

  return server;
};
