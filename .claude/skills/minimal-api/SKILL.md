---
name: minimal-api
description: 'Specify the Asp.Net Core 9.0 minimal API architecture and development URLs.'
version: 1.0.0
language: C#
framework: .NET 9+
---

## Minimal API Architecture

The architecture of the minimal API is based on the vertical slice pattern, where each feature is organized in its own folder and contains all the necessary components for that feature. This approach promotes separation of concerns and makes it easier to maintain and scale the application.

### Endpoint interface

To implement the minimal API approach, we will define an `IEndpoint` interface that all endpoint classes will implement. This interface will ensure that each endpoint has a consistent structure and can be easily registered in the application.

```csharp
public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}
```

### Endpoint classes example

For example, for the `Address` feature, we will have an `AddressEndpoint` class that implements the `IEndpoint` interface. This class will define the routes and handlers for the address-related operations.

````csharp
internal sealed class GetAddressByIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("addresses/{id:guid}", async (
            Guid id,
            ...) =>
        {
           ... // Handler logic to get address by id
        })
        .WithTags("Address")
        .RequireAuthorization();
    }
}

### Endpoint adding and mapping extension
To register and map all the endpoint classes in the application, we will create an extension method for `IServiceCollection` that will scan the assembly for all classes that implement the `IEndpoint` interface and register them.

```csharp
public static class EndpointExtensions
{
    public static IServiceCollection AddEndpoints(this IServiceCollection services)
    {
        var endpointTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => typeof(IEndpoint).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

        foreach (var endpointType in endpointTypes)
        {
            services.AddSingleton(typeof(IEndpoint), endpointType);
        }

        return services;
    }

    public static IApplicationBuilder MapEndpoints(
      this WebApplication app,
      RouteGroupBuilder? routeGroupBuilder = null)
    {
        IEnumerable<IEndpoint> endpoints = app.Services.GetRequiredService<IEnumerable<IEndpoint>>();

        IEndpointRouteBuilder builder = routeGroupBuilder is null ? app : routeGroupBuilder;

        foreach (IEndpoint endpoint in endpoints)
        {
            endpoint.MapEndpoint(builder);
        }

        return app;
    }

    public static RouteHandlerBuilder HasPermission(this RouteHandlerBuilder app, string permission)
    {
        return app.RequireAuthorization(permission);
    }
}
````

### Ask the to the user where to put the EndpointExtensions class

The `EndpointExtensions` class can be placed in a new folder called `Extensions` within the project structure. This folder will contain all extension methods that are used throughout the application, including those for configuring services and mapping endpoints.
