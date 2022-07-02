import { Entity, Schema } from 'redis-om';
import { getClient } from '../services/redis';

class Token extends Entity {}

const tokenSchema = new Schema(Token, {
    token: { type: 'string' },
});


export const getTokenRepository = async () => {
    const client = await getClient();
    return client.fetchRepository(tokenSchema);
}