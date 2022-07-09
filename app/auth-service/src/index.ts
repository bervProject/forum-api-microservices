import express from "express";
import cors from 'cors';
import { router as authRouter } from "./routes/auth";
import { getTokenRepository } from "./entities/token";
import logger from "./logger";

async function initDB() {
  const tokenRepository = await getTokenRepository();
  await tokenRepository.createIndex();
}

const port = parseInt(process.env.PORT || "9000");
const app = express();

app.use(cors());
app.use(express.json());
app.use((req, res, next) => {
  const requestMeta = {
    body: req.body,
    headers: req.headers,
    ip: req.ip,
    method: req.method,
    url: req.url,
    hostname: req.hostname,
    query: req.query,
    params: req.params,
  };

  logger.info("Getting Request", requestMeta);
  next();
});

app.get("/", (req, res) => {
  res.json({
    message: "Hello World!",
  });
});

app.use("/auth", authRouter);

app.listen(port, () => {
  initDB().then(() => {
    logger.info(`NODE_ENV: ${process.env.NODE_ENV}`);
    logger.info(`Server listen on port ${port}`);
  });
});
