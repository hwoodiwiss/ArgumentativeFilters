using ArgumentativeFilters.CodeGeneration.Parameters;

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ArgumentativeFilters.Parsing;

public static class ParameterFactory
{

    private const string AspNetCoreEndpointFilterDelegateName = "Microsoft.AspNetCore.Http.EndpointFilterDelegate";
    private const string AspNetCoreEndpointFilterInvocationContextName = "Microsoft.AspNetCore.Http.EndpointFilterInvocationContext";
    public static ArgumentativeFilterParameterProvider GetParameterCodeProvider(ParameterSyntax parameterSyntax, Compilation compilation)
    {
        var semanticModel = compilation.GetSemanticModel(parameterSyntax.SyntaxTree);

        if (semanticModel.GetDeclaredSymbol(parameterSyntax) is not IParameterSymbol parameterSymbol)
        {
            throw new ApplicationException("failed");
        }
        
        if (parameterSymbol.Type?.ToDisplayString() == AspNetCoreEndpointFilterInvocationContextName)
            return new EndpointFilterInvocationContextFilterParameter();
        
        if (parameterSymbol.Type?.ToDisplayString() == AspNetCoreEndpointFilterDelegateName)
            return new EndpointFilterDelegateFilterParameter();
        
        foreach (var attributeSymbol in parameterSymbol.GetAttributes())
        {
            if(attributeSymbol is null || attributeSymbol.AttributeClass is null) continue;
        
            INamedTypeSymbol attributeContainingTypeSymbol = attributeSymbol.AttributeClass;
            string fullName = attributeContainingTypeSymbol.ToDisplayString();
        
            if (fullName == "ArgumentativeFilters.IndexOfAttribute")
            {
                return new IndexArgumentFilterParameter(attributeSymbol.ConstructorArguments.FirstOrDefault().Value as string ?? "Invalid IndexOfAttribute parameter.");
            }
            
            if (fullName == "Microsoft.AspNetCore.Mvc.FromServicesAttribute")
            {
                return new ServiceArgumentFilterParameter(parameterSyntax.Identifier.Text, GetFullyQualifiedTypeName(parameterSymbol.Type), parameterSymbol.IsRequired());
            }
        }

        return new ValueArgumentFilterParameter(parameterSyntax.Identifier.Text, GetFullyQualifiedTypeName(parameterSymbol.Type));
    }
    
    private static bool IsRequired(this IParameterSymbol parameterSymbol) =>
        parameterSymbol.NullableAnnotation != NullableAnnotation.None 
            ? parameterSymbol.NullableAnnotation == NullableAnnotation.NotAnnotated 
            : !parameterSymbol.IsOptional;

    private static string GetFullyQualifiedTypeName(ITypeSymbol? typeSymbol)
    {
        if (typeSymbol is null) throw new ArgumentNullException(nameof(typeSymbol));
        
        string prefix = typeSymbol.SpecialType == SpecialType.None ? "global::" : string.Empty;
        string typeName = typeSymbol.NullableAnnotation == NullableAnnotation.None
            ? typeSymbol.ToDisplayString()
            : typeSymbol.ToDisplayString().Replace("?", string.Empty);
        
        return $"{prefix}{typeName}";
    }
}