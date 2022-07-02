import { Client } from "redis-om";
const connectionString = process.env.REDIS_CONNECTION_STRING;

export const getClient = async () => {
   return await new Client().open(connectionString);
}