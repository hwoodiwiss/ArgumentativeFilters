# Argumentative Filters

In classic ASP.Net APIs, `IActionFilter.ActionExecutingContext` provides access to `IDictionary<string, object?> ActionArguments` which is a name-value collection of the parameters that will be provided to the Controller action that will be run.

There isn't a direct equivalent to that in AspNetCore Minimal APIs, instead, an `EndpointFilterInvocationContext` is passed, which instead has in `IList<object?> Arguments` for accessing the Minimal API endpoint delegate argument values.

To be able to access arguments by name, you instead need to create a filter factory, which is passed an `EndpointFilterFactoryContext` which contains a `MethodInfo` representing the endpoint delegate, from which you can retrieve the parameter position of the parameters by name, and use that to index into the `Arguments` collection.

If that sounds a bit labour intensive, that's because it is. That's where Argumentative Filters come in.

With Argumentative Filters, you can create a filter that takes an `EndpointFilterInvocationContext` and an `EndpointFilterDelegate` as required parameters, then you can add whichever parameters you want that match the names of the endpoint delegate parameters, and Argumentative Filters will automatically populate those parameters with the values from the `Arguments` collection.

You can also use the `[IndexOf(ArgumentName)]` attribute on parameters of `int` type, and the index of the named argument will be provided. This allows use cases where an Argument needs to be modified in-place before being passed to the endpoint delegate.

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
