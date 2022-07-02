FROM node:lts-alpine as build
WORKDIR /app
COPY package.json yarn.lock ./
RUN yarn --frozen-lockfile
COPY src/ .
COPY tsconfig.json .
RUN yarn compile

FROM node:lts-alpine as runtime
WORKDIR /app
COPY --from=build /app/node_modules/ /app/node_modules
COPY --from=build /app/lib /app/lib
COPY package.json .
ENTRYPOINT ["npm", "start"]