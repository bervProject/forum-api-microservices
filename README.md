# forum-api-microservices

Forum API Microservices

## Directory Structure

* [/app](/app/) : Microservices Source Code
    * Currently we have [Auth Service](/app/auth-service/), [User Service](/app/user-service/), and [Thread Service](/app/thread-service/).
* [docker-compose.yml](docker-compose.yml) : Containerize MongoDB & Redis. Will help for development.

## Microservices Development

* You will need to copy or modify `docker-compose.yml` to ignore the deployment of microservices.
* Run Redis & MongoDB using `docker compose up -d`.
* Go to the microservice you want to update and read the README.md of each directory to understand how to run them.

## Development

* Build Images of Microservices: `docker compose build`
* Run all: `docker compose up -d`

## Software Architecture

![Software Architecture](docs/imgs/arch.jpeg)

## Vagrant Sample

### Provider - Virtual Box

```mermaid
architecture-beta
    group vm(server)[Virtual Machine]

    group docker(cloud)[Docker] in vm

    service sv1(server)[Service 1] in docker
    service sv2(server)[Service 2] in docker
    service db1(database)[Database 1] in docker
    service db2(database)[Database 2] in docker
    service redis(database)[Cache] in docker

    sv1:B -- T:db1
    sv2:B -- T:db2
    sv1:R -- L:redis
    sv2:L -- R:redis
```

### Docker without Vagrant

```mermaid
architecture-beta

    group docker(cloud)[Docker]

    service sv1(server)[Service 1] in docker
    service sv2(server)[Service 2] in docker
    service db1(database)[Database 1] in docker
    service db2(database)[Database 2] in docker
    service redis(database)[Cache] in docker

    sv1:B -- T:db1
    sv2:B -- T:db2
    sv1:R -- L:redis
    sv2:L -- R:redis
```


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