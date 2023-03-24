using ArgumentativeFilters.CodeGeneration.Parameters;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ArgumentativeFilters.Parsing;

public static class ParameterFactory
{

    private const string AspNetCoreEndpointFilterDelegateName = "Microsoft.AspNetCore.Http.EndpointFilterDelegate";
    private const string AspNetCoreEndpointFilterInvocationContextName = "Microsoft.AspNetCore.Http.EndpointFilterInvocationContext";
    public static ArgumentativeFilterParameterProvider GetParameterCodeProvider(ParameterSyntax parameterSyntax, Compilation compilation)
    {
        var semanticModel = compilation.GetSemanticModel(parameterSyntax.SyntaxTree);

        if (ModelExtensions.GetDeclaredSymbol(semanticModel, parameterSyntax) is not IParameterSymbol parameterSymbol)
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
                return new IndexArgumentFilterParameter(attributeSymbol.ConstructorArguments.First().Value as string ?? "failed");
            }
            
            if (fullName == "Microsoft.AspNetCore.Mvc.FromServicesAttribute")
            {
                return new ServiceArgumentFilterParameter(parameterSyntax.Identifier.Text, parameterSyntax.Type!.ToUnannotatedString(), parameterSymbol.IsRequired());
            }
        }

        return new ValueArgumentFilterParameter(parameterSyntax.Identifier.Text, parameterSyntax.Type!.ToUnannotatedString());
    }
    
    private static bool IsRequired(this IParameterSymbol parameterSymbol) =>
        parameterSymbol.NullableAnnotation != NullableAnnotation.None 
            ? parameterSymbol.NullableAnnotation == NullableAnnotation.Annotated 
            : parameterSymbol.IsOptional;
    
    private static string ToUnannotatedString(this TypeSyntax typeSyntax) =>
        typeSyntax.IsKind(SyntaxKind.NullableType) ? typeSyntax.ToString().Replace("?", string.Empty) : typeSyntax.ToString();
}