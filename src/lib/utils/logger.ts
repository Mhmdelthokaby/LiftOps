type LogLevel = "debug" | "info" | "warn" | "error";

interface LogEntry {
  timestamp: string;
  level: LogLevel;
  message: string;
  context?: string;
  data?: unknown;
}

class Logger {
  private format(entry: LogEntry): string {
    return JSON.stringify(entry);
  }

  private log(level: LogLevel, message: string, context?: string, data?: unknown) {
    const entry: LogEntry = { timestamp: new Date().toISOString(), level, message, context, data };
    const formatted = this.format(entry);

    switch (level) {
      case "debug":
        process.env.NODE_ENV === "development" && console.debug(formatted);
        break;
      case "info":
        console.log(formatted);
        break;
      case "warn":
        console.warn(formatted);
        break;
      case "error":
        console.error(formatted);
        break;
    }
  }

  debug(message: string, context?: string, data?: unknown) { this.log("debug", message, context, data); }
  info(message: string, context?: string, data?: unknown) { this.log("info", message, context, data); }
  warn(message: string, context?: string, data?: unknown) { this.log("warn", message, context, data); }
  error(message: string, context?: string, data?: unknown) { this.log("error", message, context, data); }
}

export const logger = new Logger();
