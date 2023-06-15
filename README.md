# Argumentative Filters

[![NuGet](https://img.shields.io/nuget/v/ArgumentativeFilters.svg?maxAge=3600)](https://www.nuget.org/packages/ArgumentativeFilters/)

> :warning: This package is currently in preview, and is subject to change.

## Background

In classic ASP.Net Core APIs, `IActionFilter.ActionExecutingContext` provides access to `IDictionary<string, object?> ActionArguments` which is a name-value collection of the parameters that will be provided to the Controller action that will be run.

There isn't a direct equivalent to that in AspNetCore Minimal APIs, instead, an `EndpointFilterInvocationContext` is passed, which instead has in `IList<object?> Arguments` for accessing the Minimal API endpoint delegate argument values.

To be able to access arguments by name, a [filter factory](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/min-api-filters?view=aspnetcore-7.0#register-a-filter-using-an-endpoint-filter-factory) must be created that takes an `EndpointFilterFactoryContext` which contains a `MethodInfo` representing the endpoint delegate, from which you can retrieve the parameter position of the parameters by name, and use that to index into the `Arguments` collection.

If that sounds a bit labour intensive, that's because it is. That's where Argumentative Filters come in.

With Argumentative Filters, a filter can be created as a static method that takes an `EndpointFilterInvocationContext` and an `EndpointFilterDelegate` as required parameters. Other kinds of parameters are also supported, documentation for these can be found [here](/docs/parameter-modifiers.md).

## Usage

**Filter**

```csharp
public static partial class ExampleFilter
{
    [ArgumentativeFilter]
    public static ValueTask<object?> NormalizeParameterCase(EndpointFilterInvocationContext context, EndpointFilterDelegate next, string parameter, [IndexOf(nameof(parameter))] int parameterIndex)
    {
        context.Arguments[parameterIndex] = parameter.ToUpperInvariant();
        return next(context);
    }
}

```

**Minimal API Route Configuration**

```csharp
app.MapGet("/my/route/{parameter}", (string parameter) => parameter)
    .AddEndpointFilterFactory(ExampleFilter.Factory);
```
