# Ecommerce

A .NET solution for learning microservices and Clean Architecture. The current solution contains the Catalog service projects; the API is still the ASP.NET Core starter sample, so `/weatherforecast` is available while catalog features are being built.

## Prerequisites

- .NET 10 SDK

Check that the SDK is installed:

```bash
dotnet --version
```

The version should be `10` or later. If the command is not found, install the .NET 10 SDK and reopen your terminal.

## Build the solution

Run these commands from the directory containing `Ecommerce.slnx`:

```bash
dotnet restore
dotnet build
```

`restore` downloads the external NuGet packages. `build` compiles the projects in the solution and their referenced projects. A successful build ends with `Build succeeded`.

## Run the Catalog API

From the repository root, run:

```bash
dotnet run --project Services/Catalog/Catalog.API/Catalog.API.csproj
```

The HTTP launch profile uses `http://localhost:5228`. While the process is running, open this URL to see the sample response:

```text
http://localhost:5228/weatherforecast
```

In Development, the OpenAPI document is available at:

```text
http://localhost:5228/openapi/v1.json
```

Stop the API with `Ctrl+C`.

## Projects and references

The Catalog service is split into projects by responsibility:

| Project | Responsibility |
| --- | --- |
| `Catalog.API` | HTTP entry point and application composition root |
| `Catalog.Application` | Use cases and application logic |
| `Catalog.Core` | Domain model and core business rules |
| `Catalog.Infrastructure` | External concerns such as persistence and integrations |

Current project references:

```text
Catalog.API            -> Catalog.Application
Catalog.API            -> Catalog.Infrastructure
Catalog.Infrastructure -> Catalog.Application
Catalog.Application    -> Catalog.Core
```

A project reference tells .NET that one project depends on another. It makes the referenced project's public types available to the dependent project, and ensures the dependency is built and included as needed. For example, the API will need its application and infrastructure projects when it handles requests and wires up implementations. The Application project can use domain types from Core through its reference.

A reference is different from listing a project in `Ecommerce.slnx`: the solution groups projects for building and IDE navigation, but does not by itself allow one project to use another project's code. `ProjectReference` entries in `.csproj` files define that dependency.

### Create projects for a new service

Run these commands from the repository root when creating a new Catalog-style service in folders that do not already contain projects. Do not rerun them over the existing Catalog project folders.

```bash
dotnet new webapi --name Catalog.API --output Services/Catalog/Catalog.API --framework net10.0
dotnet new classlib --name Catalog.Application --output Services/Catalog/Catalog.Application --framework net10.0
dotnet new classlib --name Catalog.Core --output Services/Catalog/Catalog.Core --framework net10.0
dotnet new classlib --name Catalog.Infrastructure --output Services/Catalog/Catalog.Infrastructure --framework net10.0
```

If these projects are not yet listed in the solution, add them from the repository root:

```bash
dotnet sln Ecommerce.slnx add Services/Catalog/Catalog.API/Catalog.API.csproj
dotnet sln Ecommerce.slnx add Services/Catalog/Catalog.Application/Catalog.Application.csproj
dotnet sln Ecommerce.slnx add Services/Catalog/Catalog.Core/Catalog.Core.csproj
dotnet sln Ecommerce.slnx add Services/Catalog/Catalog.Infrastructure/Catalog.Infrastructure.csproj
```

### Add project references

Run these commands from the repository root to establish the current dependency graph:

```bash
dotnet add Services/Catalog/Catalog.API/Catalog.API.csproj reference Services/Catalog/Catalog.Application/Catalog.Application.csproj
dotnet add Services/Catalog/Catalog.API/Catalog.API.csproj reference Services/Catalog/Catalog.Infrastructure/Catalog.Infrastructure.csproj
dotnet add Services/Catalog/Catalog.Infrastructure/Catalog.Infrastructure.csproj reference Services/Catalog/Catalog.Application/Catalog.Application.csproj
dotnet add Services/Catalog/Catalog.Application/Catalog.Application.csproj reference Services/Catalog/Catalog.Core/Catalog.Core.csproj
```

Only add a reference when the project needs to use that project's code. Keep dependencies pointing in the intended direction: Core should not depend on Application, Infrastructure, or the API. As the service grows, the API can use Infrastructure to register concrete implementations while Application depends on abstractions and Core owns the domain rules.
