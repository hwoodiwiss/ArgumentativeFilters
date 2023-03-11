using ArgumentativeFilters;

namespace ExampleMinimalApi;

public static partial class ExampleFilter
{
    public static EndpointFilterDelegate FactoryReference(EndpointFilterFactoryContext context, EndpointFilterDelegate next)
    {
        int? countryIndex = context.GetArgumentIndex("country");

        if (countryIndex.HasValue)
        {
            return invocationFilterContext =>
            {
                string country = invocationFilterContext.GetArgument<string>(countryIndex.Value);
                return NormalizeRouteStringsFilter(invocationFilterContext, next, country, countryIndex.Value);
            };
        }

        return next;
    }
    
    
    [ArgumentativeFilter]
    public static ValueTask<object?> NormalizeRouteStringsFilter(EndpointFilterInvocationContext context, EndpointFilterDelegate next, string country, [IndexOf(nameof(country))] int countryIndex)
    {
        context.Arguments[countryIndex] = country.ToUpperInvariant();
        return next(context);
    }
}