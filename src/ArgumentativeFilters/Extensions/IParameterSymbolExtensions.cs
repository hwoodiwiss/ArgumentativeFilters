namespace ArgumentativeFilters.Extensions;

public static class IParameterSymbolExtensions
{
    public static bool IsParameterRequired(this IParameterSymbol parameterSymbol)
        => parameterSymbol.NullableAnnotation != NullableAnnotation.None 
            ? parameterSymbol.NullableAnnotation == NullableAnnotation.NotAnnotated 
            : !parameterSymbol.IsOptional;

    // For contexts without NRTs, we follow the behaviour of ASP.NET Core's [FromServices] attribute for optionality.
    public static bool IsServiceRequired(this IParameterSymbol parameterSymbol) 
        => parameterSymbol.NullableAnnotation != NullableAnnotation.None 
            ? parameterSymbol.NullableAnnotation == NullableAnnotation.NotAnnotated 
            : !parameterSymbol.IsOptional;
}