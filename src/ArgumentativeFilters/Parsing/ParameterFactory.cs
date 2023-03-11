using ArgumentativeFilters.CodeGeneration.Parameters;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using CSharpExtensions = Microsoft.CodeAnalysis.CSharp.CSharpExtensions;

namespace ArgumentativeFilters.Parsing;

public static class ParameterFactory
{

    private const string AspNetCoreEndpointFilterDelegateName = "Microsoft.AspNetCore.Http.EndpointFilterDelegate";
    private const string EndpointFilterDelegateName = "EndpointFilterDelegate";
    private const string AspNetCoreEndpointFilterInvocationContextName = "Microsoft.AspNetCore.Http.EndpointFilterInvocationContext";
    private const string EndpointFilterInvocationContextName = "EndpointFilterInvocationContext";
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
        }

        return new ValueArgumentFilterParameter(parameterSyntax.Identifier.Text, parameterSyntax.Type!.ToString());
        
        // foreach (var attributeList in parameterSyntax.AttributeLists)
        // {
        //     foreach (var attribute in attributeList.Attributes)
        //     {
        //         var attributeSymbol = semanticModel.GetSymbolInfo(attribute).Symbol as IMethodSymbol;
        //
        //         if (attributeSymbol is null)
        //         {
        //             continue;
        //         }
        //         
        //         INamedTypeSymbol attributeContainingTypeSymbol = attributeSymbol.ContainingType;
        //         string fullName = attributeContainingTypeSymbol.ToDisplayString();
        //
        //         if (fullName == "ArgumentativeFilters.IndexOfAttribute")
        //         {
        //             return new IndexArgumentFilterParameter("country");
        //         }
        //     }
        // }
        //
        //
        // return new ValueArgumentFilterParameter(parameterSyntax.Identifier.Text, parameterSyntax.Type!.ToString());
    }
}