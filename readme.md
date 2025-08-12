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

### Manually

# License

I don't give a damn thing, take it and get out here.