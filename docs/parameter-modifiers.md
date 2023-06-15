# Parameter Modifiers

## IndexOf

The `IndexOfAttribute` can be used to have the index of an endpoint parameter.

To do this, add the `IndexOfAttribute` to a parameter of your filter, with the name of the endpoint parameter you need the index for.

```csharp
[IndexOf(nameof(parameter))] int parameterIndex
```

This can be useful for cases where the the filter is going to be used to mutate endpoint parameters, as you will need the index to insert the updated version back into `EndpointFilterInvocationContext.Arguments`.

> *Note :notebook:*
> - If the annotated index parameter is nullable and no parameter with the given name exists your filter will still be run, and passed null for this parameter.
> - If the annotated index parameter is *not* nullable and no parameter with the given name exists you're filter will be skipped.

## FromServices

This source generator has special handling for parameters annotated with `Microsoft.AspNetCore.Mvc.FromServicesAttribute`.

```csharp
[FromServices] IMyService myService
```

When a parameter has this attribute, the generated factory will attempt to resolve an instance of the parameter type from the `HttpContext.RequestServices` scoped service provider.

> *Note :notebook:*
> - In nullable reference type contexts if the parameter is marked as nullable, `IServiceProvider.GetService` is used, otherwise `IServiceProvider.GetRequiredService` is used, and the factory will throw if the service cannot be resolved.
> - In contexts where nullable reference types are not enabled `IServiceProvider.GetRequiredService` will be used unless the parameter is _optional_.