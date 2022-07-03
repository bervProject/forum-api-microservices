# Auth Service

## Setup

* Run `yarn` or `npm install`

## Linter

* Run `yarn lint`. To fix: `yarn lint --fix`.

## Development Mode

* Setup `env`. If you are using Linux or MacOS, you may copy `.env.example` to `.env.sh` and run `source .env.sh` to load the environment.
* Run `yarn start:dev` or `npm run start:dev`

## Run in Production

* Build `.js` file using `yarn compile` or `npm run compile`
* Setup `env`. If you are using Linux or MacOS, you may copy `.env.example` to `.env.sh` and run `source .env.sh` to load the environment.
* Run `yarn start`. You may use `pm2` as alternative to run in production and setup the env. Learn more [here](https://pm2.keymetrics.io/docs/usage/quick-start/).

**Note**: You may use `Dockerfile` if you wish to host this service in containerize environment.

## License

MIT

```
MIT License

Copyright (c) 2022 Bervianto Leo Pratama's Personal Projects

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```
