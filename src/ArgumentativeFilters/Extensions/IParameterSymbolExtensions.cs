namespace ArgumentativeFilters.Extensions;

public static class IParameterSymbolExtensions
{
    public static bool IsNullable(this IParameterSymbol parameterSymbol) 
        => parameterSymbol.NullableAnnotation != NullableAnnotation.None
            ? parameterSymbol.NullableAnnotation == NullableAnnotation.Annotated
            : parameterSymbol.Type switch
            {
                { IsValueType: true, OriginalDefinition.SpecialType: SpecialType.System_Nullable_T} => true,
                { IsValueType: true } => false,
                _ => true,
            };

    // For contexts without NRTs, we follow the behaviour of ASP.NET Core's [FromServices] attribute for optionality.
    public static bool IsServiceRequired(this IParameterSymbol parameterSymbol) 
        => parameterSymbol.NullableAnnotation != NullableAnnotation.None 
            ? parameterSymbol.NullableAnnotation == NullableAnnotation.NotAnnotated 
            : !parameterSymbol.IsOptional;
}