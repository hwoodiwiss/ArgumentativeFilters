namespace ArgumentativeFilters.Generator.Extensions;

public static class IMethodSymbolExtensions
{
    public static string? GetAttributeNamedParameterValue(this IMethodSymbol methodSymbol, string attributeName, string parameterName)
    {
        foreach (var attribute in methodSymbol.GetAttributes())
        {
            if (attribute.AttributeClass?.ToDisplayString() == attributeName)
            {
                foreach (var pair in attribute.NamedArguments)
                {
                    if (pair.Key == parameterName)
                    {
                        if (pair.Value.Value is not null)
                        {
                            return pair.Value.Value.ToString();
                        }
                    }
                }
            }
        }

        return null;
    }
}