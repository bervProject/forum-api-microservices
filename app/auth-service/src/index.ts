import express from "express";
import { router as authRouter } from "./routes/auth";
import { getTokenRepository } from "./entities/token";

async function initDB() {
const tokenRepository = await getTokenRepository();
  await tokenRepository.createIndex();
}

const port = parseInt(process.env.PORT || "9000");
const app = express();
app.use(express.json());

app.get("/", (req, res) => {
  res.json({
    message: "Hello World!",
  });
});

app.use("/auth", authRouter);

app.listen(port, () => {
  initDB().then(() => {
    console.log(`Server listen on port ${port}`);
  });
});
