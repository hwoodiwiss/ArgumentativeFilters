using ArgumentativeFilters.CodeGeneration.Parameters;

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ArgumentativeFilters.Parsing;

public static class ParameterCodeProviderFactory
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
            if(attributeSymbol?.AttributeClass is null) continue;
        
            INamedTypeSymbol attributeContainingTypeSymbol = attributeSymbol.AttributeClass;
            string fullName = attributeContainingTypeSymbol.ToDisplayString();
        
            if (fullName == "ArgumentativeFilters.IndexOfAttribute")
            {
                return new IndexArgumentFilterParameter(attributeSymbol.ConstructorArguments.FirstOrDefault().Value as string ?? "Invalid IndexOfAttribute parameter.", parameterSymbol.IsRequired());
            }
            
            if (fullName == "Microsoft.AspNetCore.Mvc.FromServicesAttribute")
            {
                return new ServiceArgumentFilterParameter(parameterSyntax.Identifier.Text, GetFullyQualifiedTypeName(parameterSymbol.Type), parameterSymbol.IsRequired());
            }
        }

        return new ValueArgumentFilterParameter(parameterSyntax.Identifier.Text, GetFullyQualifiedTypeName(parameterSymbol.Type), parameterSymbol.IsRequired());
    }
    
    private static bool IsRequired(this IParameterSymbol parameterSymbol) =>
        parameterSymbol.NullableAnnotation != NullableAnnotation.None 
            ? parameterSymbol.NullableAnnotation == NullableAnnotation.NotAnnotated 
            : !parameterSymbol.IsOptional;

    private static string GetFullyQualifiedTypeName(ITypeSymbol? typeSymbol)
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
        => $"global::{GetTypeNameWithStrippedNullability(typeSymbol)}:{typeSymbol.SpecialType}";

    private static string GetTypeNameWithStrippedNullability(ITypeSymbol typeSymbol) =>
        typeSymbol.NullableAnnotation == NullableAnnotation.None
            ? typeSymbol.ToDisplayString()
            : typeSymbol.ToDisplayString().Replace("?", string.Empty);
}