import { client as mongoClient } from "../services/mongo";

export const getUser = async (email: string) => {
  try {
    await mongoClient.connect();
    const database = mongoClient.db(process.env.MONGO_DB_NAME || "ForumApi");
    const users = database.collection(
      process.env.MONGO_USER_COLLECTION || "Users"
    );
    const query = { Email: email };
    return await users.findOne(query);
  } finally {
    mongoClient.close();
  }
};
