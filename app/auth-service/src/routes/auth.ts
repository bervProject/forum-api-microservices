import { Router } from "express";
import argon2 from "argon2";
import { JwtPayload } from "jsonwebtoken";
import { getTokenRepository } from "../entities/token";
import { generateToken, verifyToken } from "../services/token";
import { getUser } from "../repositories/user";
import constants from "../constants";
import logger from "../logger";

const failedLoginMessage = {
  message: "Failed to Login",
};

const failedLogoutMessage = {
  message: "Failed to Logout",
};

const commonFailed = {
  message: "Failed",
};

type SuccessLogin = {
  message: string;
  accessToken?: string;
  refreshToken?: string;
};

export const router = Router();

router.post("/login", async (req, res) => {
  const email = req.body.email;
  const password = req.body.password;
  if (!email || !password) {
    res.statusCode = 401;
    res.json(failedLoginMessage);
    return;
  }
  const existingUser = await getUser(email);
  if (existingUser == null) {
    res.statusCode = 401;
    res.json(failedLoginMessage);
    return;
  }
  const existingPass = existingUser.Password;
  try {
    const verify = await argon2.verify(existingPass, password);
    if (!verify) {
      res.statusCode = 401;
      res.json(failedLoginMessage);
      return;
    }
  } catch (err) {
    logger.error("Error when verify token", err);
    res.statusCode = 401;
    res.json(failedLoginMessage);
    return;
  }

  const userID = existingUser._id.toString("hex");
  const payload = {
    id: userID,
    name: existingUser.Name,
    email: existingUser.Email,
  };
  const accessToken = generateToken(payload, "ACCESS_TOKEN");
  const refreshToken = generateToken(payload, "REFRESH_TOKEN");
  const tokenRepository = await getTokenRepository();
  const createdToken = await tokenRepository.createAndSave({
    token: refreshToken,
  });
  await tokenRepository.expire(
    createdToken.entityId,
    constants.defaultExpiredSecond
  );
  const successMessage: SuccessLogin = {
    message: "Success",
  };
  successMessage["accessToken"] = accessToken;
  successMessage["refreshToken"] = refreshToken;
  res.json(successMessage);
});

router.post("/refresh", async (req, res) => {
  const token = req.body.token;
  if (!token) {
    res.statusCode = 400;
    res.json(commonFailed);
    return;
  }
  try {
    const payload = verifyToken(token, "REFRESH_TOKEN");
    const tokenRepository = await getTokenRepository();
    const tokenId = await tokenRepository
      .search()
      .where("token")
      .equals(token)
      .return.firstId();
    if (tokenId == null) {
      res.statusCode = 400;
      res.json(failedLogoutMessage);
      return;
    }
    const data = payload as JwtPayload;
    const { id, name, email } = data;
    res.json({
      accessToken: generateToken({ id, name, email }, "ACCESS_TOKEN"),
    });
    return;
  } catch (err) {
    logger.error("Error when verify token", err);
    res.statusCode = 401;
    res.json(commonFailed);
    return;
  }
});

router.post("/verify", async (req, res) => {
  const token = req.body.token;
  if (!token) {
    res.statusCode = 400;
    res.json(commonFailed);
    return;
  }
  try {
    const payload = verifyToken(token, "ACCESS_TOKEN");
    const data = payload as JwtPayload;
    const { id } = data;
    res.json({
      message: "OK",
      id,
    });
    return;
  } catch (err) {
    logger.error("Error when verify token", err);
    res.statusCode = 401;
    res.json(commonFailed);
    return;
  }
});

router.post("/logout", async (req, res) => {
  const token = req.body.token;
  if (!token) {
    res.statusCode = 400;
    res.json(failedLogoutMessage);
    return;
  }
  const tokenRepository = await getTokenRepository();
  const tokenId = await tokenRepository
    .search()
    .where("token")
    .equals(token)
    .return.firstId();
  if (tokenId == null) {
    res.statusCode = 400;
    res.json(failedLogoutMessage);
    return;
  }
  tokenRepository.remove(tokenId);
  res.json({ message: "Success Logout" });
});
