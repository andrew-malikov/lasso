# Users and Finance Services

<!-- TOC -->
* [Users and Finance Services](#users-and-finance-services)
* [What's it all about](#whats-it-all-about)
  * [Run](#run)
    * [Docker and Docker Compose](#docker-and-docker-compose)
    * [Manually](#manually)
* [License](#license)
<!-- TOC -->

# What's it all about

There are two services:

- Users
- Finance

Users service allows to register a user and perform login/ logout. The authentication part is made on top of JWT.

Finance provides currency rates by a user. It fills the currency via different mircoservice.

Postgres is shared between the services, though has microservice access it's own database. Migrations are managed by
separte runnable services. Check the docker files and compose.

## Run

### Docker and Docker Compose

```sh
docker compose up --build --force-recreate
```

And head directly to `http//localhost:8080/swagger` to try endpoints. That thing is API gateway and it handovers requests to GRPC microservices.
Call to register a user at first, `/api/users/register` and then get the tokens via `/api/users/login`. Grab the `access_token` and proceed to `/api/currencies/favourites`. After all the steps hit `/api/users/logout` with the refresh token.
That's all.

### Manually

Well, you'll need to do a lot. I don't have time to write it all down. 

# License

I don't give a damn thing, take it and get out here.