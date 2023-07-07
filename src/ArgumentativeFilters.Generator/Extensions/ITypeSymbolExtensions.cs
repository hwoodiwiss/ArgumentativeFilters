namespace ArgumentativeFilters.Generator.Extensions;

public static class ITypeSymbolExtensions
{
    public static string GetFullyQualifiedTypeName(this ITypeSymbol? typeSymbol)
    {
        if (typeSymbol is null) throw new ArgumentNullException(nameof(typeSymbol));

        return typeSymbol switch
        {
            { OriginalDefinition.SpecialType: SpecialType.System_Nullable_T } => GetValueTypeName(typeSymbol),
            { SpecialType: SpecialType.None } => GetGloballyQualifiedTypeName(typeSymbol),
            _ => GetTypeNameWithStrippedNullability(typeSymbol),
        };
    }

    private static string GetValueTypeName(ITypeSymbol typeSymbol)
        => $"global::System.Nullable<{GetTypeNameWithStrippedNullability(typeSymbol)}>";

    private static string GetGloballyQualifiedTypeName(ITypeSymbol typeSymbol)
        => $"global::{GetTypeNameWithStrippedNullability(typeSymbol)}";

    private static string GetTypeNameWithStrippedNullability(ITypeSymbol typeSymbol) =>
        typeSymbol.NullableAnnotation == NullableAnnotation.None
            ? typeSymbol.ToDisplayString()
            : typeSymbol.ToDisplayString().Replace("?", string.Empty);
}