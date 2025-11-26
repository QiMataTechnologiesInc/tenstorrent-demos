const LEVELS = {
  error: 0,
  warn: 1,
  info: 2,
  debug: 3,
};

const parseLevel = (input) => {
  if (typeof input === "number") {
    return input;
  }

  const normalized = typeof input === "string" ? input.trim().toLowerCase() : "info";
  return LEVELS[normalized] ?? LEVELS.info;
};

const resolveLevel = (overrideLevel) => {
  if (overrideLevel !== undefined && overrideLevel !== null) {
    return parseLevel(overrideLevel);
  }

  const envLevel = typeof process !== "undefined" ? process.env?.TT_LOG_LEVEL : undefined;
  return parseLevel(envLevel);
};

const createMethod = (threshold, levelName, namespace) => {
  const method = console[levelName] ? console[levelName].bind(console) : console.log.bind(console);
  const levelValue = LEVELS[levelName];
  return (message, extra = {}) => {
    if (levelValue > threshold) {
      return;
    }
    const prefix = namespace ? `[${namespace}]` : "";
    method(`${prefix}[${levelName.toUpperCase()}] ${message}`, extra);
  };
};

export const createLogger = (namespace, level) => {
  const resolvedLevel = resolveLevel(level);
  return {
    level: resolvedLevel,
    debug: createMethod(resolvedLevel, "debug", namespace),
    info: createMethod(resolvedLevel, "info", namespace),
    warn: createMethod(resolvedLevel, "warn", namespace),
    error: createMethod(resolvedLevel, "error", namespace),
  };
};

export const levels = { ...LEVELS };
