import jwt from "jsonwebtoken";
import constants from "../constants";

const secretToken = process.env.AUTH_SECRET || "auth_secret_123";
const refreshTokenSecret = process.env.REFRESH_SECRET || "refresh_secret_123";

type TokenType = "ACCESS_TOKEN" | "REFRESH_TOKEN";

const getSecret = (tokenType: TokenType) => {
  return tokenType == "ACCESS_TOKEN" ? secretToken : refreshTokenSecret;
};

const getExp = (tokenType: TokenType) => {
  return tokenType == "ACCESS_TOKEN" ? constants.defaultExpired : constants.defaultExpiredRefresh;
};

export const generateToken = (
  {
    id,
    name,
    email,
  }: {
    id: string;
    name: string;
    email: string;
  },
  tokenType: TokenType
) => {
  const secret = getSecret(tokenType);
  return jwt.sign({ id, name, email }, secret, {
    expiresIn: getExp(tokenType),
  });
};

export const verifyToken = (token: string, tokenType: TokenType) => {
  const secret = getSecret(tokenType);
  return jwt.verify(token, secret);
};

export const decode = (token: string) => {
  return jwt.decode(token);
};
