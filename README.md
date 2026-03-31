# movie-info
Small movie information querying app using info from the OMDB API. Made by Ed Younskevicius (solamen2 AT gmail). NOTE: Basic skeleton app has been coded and deployed to two different platforms, but funcionality is still WIP.

## Using The App
If you'd like to see how the app works eventually (though it's not working yet), please go to [movieinfo.dev](https://movieinfo.dev) and use these credentials:

Username: TODO **NO USERNAME YET**
Password: TODO **NO PASSWORD YET**

This account will be heavily rate-limited and will not allow you to configure which fields are returned, but hopefully will be working for you to try eventually. (When there's a a real username and password above, and it's not working, feel free to send me an email and I'll investigate.)

## AI Usage During Development
I did not use AI at all during the setup of the project, since I wanted to have a very good understanding of the basic architecture of the app and the technical tradeoffs I was making. After setting up the basic skeleton and backend, I have been using Claude Code to help me code the frontend React pages quickly, since I have some experience with React already. (I go over every change it makes by hand, and sometimes re-prompt or make small changes myself.) Anytime AI was used, I have included the prompts as a comment on the PR that merged those changes.

## Architecture: Backend

### API
The backend API is an ASP.Net Core 10.0.2 app (written in C# 14+, of course), using [minimal APIs](https://learn.microsoft.com/en-us/aspnet/core/tutorials/min-web-api?view=aspnetcore-10.0&tabs=visual-studio).

### Deployment
The app is deployed using Docker (both the frontend and backend live in a single Docker image, because the frontend is copied as static files into wwwroot). Currently it is running as an Azure Container App and pulls the image from Docker Hub. (I've used Azure Container Registry before, but I wanted to learn to use Docker Hub for this project.) I've also deployed it via Docker on fly.io (to learn the platform and test ease of multi-platform deployment), which dictated several other architecture decisions described later. (For example, I've intentionally avoided using Azure Container Registry for the app registry, and avoided using a docker-compose.yaml file (and also avoided using multiple containers), since I wanted to be able to deploy to [fly.io](https://fly.io/docs/languages-and-frameworks/dotnet/) or other services. fly.io does not currently support Docker Compose well, and also, not using Docker Compose also has the benefit of keeping the app simple as a single image.)

A somewhat unusual feature of the app is that I build the Docker image using [Docker multi-platform builds](https://docs.docker.com/build/building/multi-platform/). Even though I'm building the image on an M-series MacBook, I specify the platform as "linux/amd64,linux/arm64" so I can build a Docker image that will work directly on my machine and Azure (and other places) from the exact same Docker image, ensuring uniformity and ease of debugging.

(Also, the fly.io version of this app does not use Docker Hub. fly.io has a [private registry](https://fly.io/docs/blueprints/using-the-fly-docker-registry/), but I find it easy enough to just [build and deploy directly from the Dockerfile on fly.io](https://fly.io/docs/languages-and-frameworks/dockerfile/).)

### Database
The app uses a SQL Server database to store user configration. This is a bit of overkill, as it could have easily been handled using Azure Cache For Redis or SQLite (both which I have used before successfully), or other technologies as well. But I wanted to teach myself how to build an app using SQL Server from the ground up, and it works well so far.

Database operations are handled using Entity Framework Core. I'm very comfortable using SQL directly, but I like EF Core, and I think it's a great fit for many apps, especially simple apps like this one.

(Two small notes: I develop this app on a Mac, so I run SQL Server locally using an official Docker image. Also, using SQL Server in Azure on fly.io, due to IP address whitelisting requirements, requires either [setting up a small proxy app](https://fly.io/docs/networking/egress-ips/#the-proxy-pattern) or [paying for a static egress IP](https://community.fly.io/t/static-egress-ips-for-machines/22004). I chose the latter, since setting up the former is more complex and probably would cost about the same per year.)

### Secrets
The secrets are stored in environment variables (and also the native secrets storage in Azure Container App and fly.io). Again, this is to facilitate using multiple services; I initially got the proof of concept working using Azure Key Vault, but I wanted to remain service-agnostic, so I changed over to using environment variables for the secrets. It would be easy to switch back in the unlikely case anyone besides me ever works on this app.

### Authentication / Authorization
Authentication uses HttpOnly cookies generated by ASP.NET Core Identity, and stores user credentials in the SQL Server db. (There's no need for more complex solutions like JWTs or OIDC in a simple, single-domain app like this one.) Authorization uses claims-based access control (which are added on using a scaffolded Core Identity user), not role-based access control as you might expect for a simple app. Again, probably a bit of overkill for the level of complexity of this app at the moment, but CBAC is often a lot easier to work with down the road as an app gets more complex, and it's tough to convert to using it later (a pain I have experienced personally). And I think the overhead isn't too bad, myself.

### Logging
Logs are handled using [Serilog](https://serilog.net/) for structured logging.

### Testing
Backend unit testing is done using [xUnit.net](https://xunit.net/?tabs=cs) with HTTP mocks provided via [Moq](https://github.com/devlooped/moq) . Ad-hoc API testing is done using [Scalar](https://scalar.com/), since [SwaggerUI / Swashbuckle was removed by default in .NET 9](https://github.com/dotnet/aspnetcore/issues/54599). As [Microsoft recommends](https://learn.microsoft.com/en-us/ef/core/testing/testing-with-the-database), end-to-end testing is done on the backend using an actual SQL Server database.

### Data Sources

TODO: Mention IMDB, OMDB, and maybe TMDB

## Architecture: Frontend

### Framework
The frontend framework is React, and the app is written using Typescript in TSX files. It was created using a basic template with Vite (version 7.3.1 currently) by running "npm create vite@latest", selecting "React" as the framework, and "Typescript" as the variant. Most of the UI was created using AI.

### Testing
Frontend unit/component/integration tests are done using [Vitest](https://vitest.dev/) as the test runner, [React Testing Library](https://testing-library.com/) and [@testing-library/jest-dom](https://github.com/testing-library/jest-dom) for the tests themselves, and [MSW](https://mswjs.io/) to mock the HTTP calls. End-to-end tests are done on the frontend using (Playwright)[https://playwright.dev/], with toggleable MSW using [mswjs/playwright](https://github.com/mswjs/playwright). (Yes, I am aware then when using MSW, it's not a true end-to-end test.) Ad-hoc UI tests are performed the same as the end-to-end tests, but by me instead of Playwright.
TODO: Talk about code coverage

### Handling Web Requests
In a local dev environment, Vite handles all requests, and proxies the API requests to the ASP.Net Core Kestrel server, which is configured using [vite.config.ts](https://vite.dev/config/server-options#server-proxy). Outside of development, the React files are copied to wwwroot (via a custom build step in the MovieInfoBackend.csproj file that runs when doing a "dotnet publish") and are therefore served statically, so in stage and prod ASP.Net Core handles all the requests instead. All traffic is over HTTPS using [Let's Encrypt](https://letsencrypt.org/) certs that renew every 3 months via [Acmebot for Microsoft Azure](https://github.com/shibayan/keyvault-acmebot). Routing is handled using [React Router](https://www.npmjs.com/package/react-router-dom).

### CSS
The CSS and presentation is fairly basic (and does not use Tailwind), and was determined mostly by AI. It uses Flexbox to display the search cards and extended data (from subsequent OMDB / TMDB searches). It does not support dark mode. TODO: Support dark mode?

### Accessibility
TODO: Make app more accessible?

## Architecture: Environments
The app has three environments: dev, stage, and prod. Dev is run on my local machine, and also on fly.io (and the fly.io version has its own Azure SQL Server database). Stage and prod are separate Azure Container Apps with their own Azure SQL Server databases.

## Architecture: CI/CD
CI/CD is performed at the PR level. All unit / integration tests (backend and frontend) must pass for a PR to be able to be merged. When the PR is merged, the app is automatically deployed to the dev environment in Fly.io and all end-to-end tests are run. TODO: What happens when end-to-end tests fail?