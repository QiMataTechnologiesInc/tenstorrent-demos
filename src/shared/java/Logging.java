package com.tenstorrent.shared;

import java.util.Locale;
import java.util.logging.ConsoleHandler;
import java.util.logging.Handler;
import java.util.logging.Level;
import java.util.logging.Logger;
import java.util.logging.SimpleFormatter;

public final class Logging {
    private static final String LEVEL_ENV = "TT_LOG_LEVEL";

    private Logging() {}

    public static Level resolveLevel(String overrideLevel) {
        String candidate = (overrideLevel == null || overrideLevel.isBlank())
                ? System.getenv(LEVEL_ENV)
                : overrideLevel;

        if (candidate == null || candidate.isBlank()) {
            return Level.INFO;
        }

        String normalized = candidate.trim().toUpperCase(Locale.ROOT);
        return switch (normalized) {
            case "TRACE", "FINEST" -> Level.FINEST;
            case "DEBUG", "FINE" -> Level.FINE;
            case "INFO", "INFORMATION" -> Level.INFO;
            case "WARN", "WARNING" -> Level.WARNING;
            case "ERROR", "SEVERE" -> Level.SEVERE;
            default -> Level.INFO;
        };
    }

    public static Logger getLogger(String name) {
        Logger logger = Logger.getLogger(name);
        Level level = resolveLevel(null);
        logger.setLevel(level);
        logger.setUseParentHandlers(false);

        boolean hasHandlers = logger.getHandlers().length > 0;
        if (!hasHandlers) {
            ConsoleHandler handler = new ConsoleHandler();
            handler.setLevel(level);
            handler.setFormatter(new SimpleFormatter());
            logger.addHandler(handler);
        } else {
            for (Handler handler : logger.getHandlers()) {
                handler.setLevel(level);
            }
        }

        return logger;
    }
}
