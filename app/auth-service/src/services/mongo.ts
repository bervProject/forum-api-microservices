import { MongoClient } from 'mongodb';

const connectionString = process.env.MONGO_CONNECTION_STRING;
export const client = new MongoClient(connectionString || "");