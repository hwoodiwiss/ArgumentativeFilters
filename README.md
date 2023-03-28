# Argumentative Filters

> :warning: This is an early preview package, and is not intended for production use yet. It is likely broken in a number of ways.

## Background

In classic ASP.Net Core APIs, `IActionFilter.ActionExecutingContext` provides access to `IDictionary<string, object?> ActionArguments` which is a name-value collection of the parameters that will be provided to the Controller action that will be run.

There isn't a direct equivalent to that in AspNetCore Minimal APIs, instead, an `EndpointFilterInvocationContext` is passed, which instead has in `IList<object?> Arguments` for accessing the Minimal API endpoint delegate argument values.

To be able to access arguments by name, a [filter factory](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/min-api-filters?view=aspnetcore-7.0#register-a-filter-using-an-endpoint-filter-factory) must be created that takes an `EndpointFilterFactoryContext` which contains a `MethodInfo` representing the endpoint delegate, from which you can retrieve the parameter position of the parameters by name, and use that to index into the `Arguments` collection.

If that sounds a bit labour intensive, that's because it is. That's where Argumentative Filters come in.

With Argumentative Filters, a filter can be created as a static method that takes an `EndpointFilterInvocationContext` and an `EndpointFilterDelegate` as required parameters. Other kinds of parameters are also supported and documented [below](#supported-parameters--modifiers).

## Roadmap/Wishlist

- [ ] Work in non-trivial cases :sweat_smile:
- [ ] Have tests
- [ ] Support nested classes
- [ ] Provide usage analyzers
- [ ] Allow specifying preferred scope for injected services (Can exclude scoped services and resolve services from `EndpointFilterFactoryContext.ApplcationServices`)
- [ ] Provide some way to define fallback behaviour if filter arguments cannot be matched

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

## Parameter Modifiers

### IndexOf

This source generator provides an IndexOfAttribute with a single constructor argument, which is the endpoint argument to populate the annotated parameter with.

```csharp
[IndexOf(nameof(parameter))] int parameterIndex
```

This can be useful for cases where the the filter is going to be used to mutate endpoint parameters.

### FromServices

This source generator has special handling for parameters annotated with the `Microsoft.AspNetCore.Mvc.FromServicesAttribute`.
When a parameter has this attribute, the generated factory will attempt to resolve an instance of the parameter type from the `HttpContext.RequestServices` scoped service provider.
Nullability and Optionallity are respected when doing this. 

> *Note :notebook:*
> - In nullable reference type contexts if the parameter is marked as nullable, `IServiceProvider.GetService` is used, otherwise `IServiceProvider.GetRequiredService` is used, and the factory will throw if the service cannot be resolved.
> - In contexts where nullable reference types are not enabled `IServiceProvider.GetRequiredService` will be used unless the parameter is _optional_.