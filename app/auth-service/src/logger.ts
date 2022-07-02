import { createLogger, format, transports } from "winston";
const { combine, timestamp, printf } = format;

const myFormat = printf(({ level, message, timestamp, ...meta }) => {
  const service = meta.service;
  delete meta.service;
  const otherMeta = JSON.stringify(meta);
  return `[${service}] ${timestamp} [${level}] ${message}. Metadata: ${otherMeta}`;
});

const logger = createLogger({
  level: "info",
  format: combine(timestamp(), myFormat),
  defaultMeta: { service: "auth-service" },
  transports: [new transports.Console()],
});

export default logger;
