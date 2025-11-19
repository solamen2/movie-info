# movie-info
Small movie information querying app using info from the OMDB API. Made by Ed Younskevicius (solamen2 AT gmail).

## Using The App
If you'd like to see how the app works eventually (though it's not working yet), please go to [movieinfo.dev](https://movieinfo.dev) and use these credentials:

Username: **NO USERNAME YET**
Password: **NO PASSWORD YET**

This account will be heavily rate-limited, but hopefully will working for you to try eventually. (When there's a a real username and password above, and it's not working, feel free to send me an email and I'll investigate.)

## AI Usage During Development
I did not use AI at all during the setup of the project, since I wanted to have a very good understanding of the basic architecture of the app and the technical tradeoffs I was making. After setting up the basic skeleton, I am planning to use Claude Code to help me code the React and .NET Core APIs quickly, since I have a good amount of experience with both already.

## Architecture: Backend

### API
The backend API is an ASP.Net Core 10 app (written in C#, of course), using [minimal APIs](https://learn.microsoft.com/en-us/aspnet/core/tutorials/min-web-api?view=aspnetcore-10.0&tabs=visual-studio).

### Deployment
The app is deployed using Docker and a Dockerfile. Currently it is running as an Azure Container App and pulls the image from Docker Hub. Soon it will also be deployed on fly.io, which dictated several other architecture decisions that I will describe here. (For example, I have intentionally avoided using Azure Container Registry for the app registry, and avoided using a docker-compose.yaml file for the image building, since I wanted to be able to deploy to [fly.io](https://fly.io/docs/languages-and-frameworks/dotnet/) or other services. fly.io does not currently support docker-compose.yaml well, though using a Dockerfile also has the benefit of keeping the app simple as a single image.)

A somewhat unusual feature of the app is that I build the Docker image using [Docker multi-platform builds](https://docs.docker.com/build/building/multi-platform/). Even though I'm building on an M-series MacBook, by specifying the platform as "linux/amd64,linux/arm64", I can build a Docker image that will work directly on my machine and Azure (and other places) from the exact same Docker image, ensuring uniformity and ease of debugging.

### Database
The app uses a SQL Server database to store user configration. This is a bit of overkill, as it could have easily been handled using Azure Cache For Redis or SQLite (both which I have used before successfully), or other technologies as well. But I wanted to teach myself how to build an app using SQL Server from the ground up, and it works well so far.

Database operations are handled using Entity Framework Core. I'm very comfortable using SQL directly, but I like EF Core, and I think it's a great fit for many apps, especially simple apps like this one.

(A small note: I develop this app on a Mac, so I run SQL Server locally using an official Docker image.)

### Secrets
The secrets are stored in environment variables (and also Secrets in the Azure Container App configuration). Again, this is to facilitate using multiple services; I initially got the proof of concept working using Azure Key Vault, but I wanted to remain service-agnostic, so I changed over to using environment variables for the secrets. It would be easy to switch back in the unlikely case anyone besides me ever works on this app.

## Architecture: Frontend

### Framework
The frontend framework is React, and the app is written using Typescript in TSX files. It was created using a basic template by running "npm create vite@latest", selecting "React" as the framework, and "Typescript" as the variant.

### Testing
TODO

### Handling Web Requests
In a local dev environment, Vite handles all requests, and proxies the API requests to the ASP.Net Core Kestrel server, which is configured using [vite.config.ts](https://vite.dev/config/server-options#server-proxy). Outside of development, the React files are copied to wwwroot (via a custom build step in the MovieInfoBackend.csproj file that runs when doing a "dotnet publish") and are therefore served statically, so in stage and prod ASP.Net Core handles all the requests instead.

## Architecture: Environments
The app has three environments: local (dev), stage, and prod. Local is only run on my local machine, and the other two are separate Azure Container Apps with their own SQL Server instances.