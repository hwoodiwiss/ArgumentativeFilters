namespace ArgumentativeFilters.Generator.CodeGeneration.Extensions;

public static class INamedTypeSymbolExtensions
{
    internal static string GetAccessibilityString(this INamedTypeSymbol namedTypeSymbol)
    {
        return namedTypeSymbol.DeclaredAccessibility switch
        {
            Accessibility.Public => "public",
            Accessibility.Internal => "internal",
            _ => throw new InvalidOperationException("Filter must be declared in a public or internal class.")
        };
    }
}