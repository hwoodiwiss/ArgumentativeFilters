using ArgumentativeFilters.Generator.CodeGeneration.Parameters;
using ArgumentativeFilters.Generator.Extensions;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ArgumentativeFilters.Generator.Parsing;

public static class ParameterCodeProviderFactory
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
            if (attributeSymbol?.AttributeClass is null) continue;

            INamedTypeSymbol attributeContainingTypeSymbol = attributeSymbol.AttributeClass;
            string fullName = attributeContainingTypeSymbol.ToDisplayString();

            if (fullName == "ArgumentativeFilters.IndexOfAttribute")
            {
                return new IndexArgumentFilterParameter(attributeSymbol.ConstructorArguments.FirstOrDefault().Value as string ?? "Invalid IndexOfAttribute parameter.", parameterSymbol.IsParameterRequired());
            }
            
            if (fullName == "Microsoft.AspNetCore.Mvc.FromServicesAttribute")
            {
                return new ServiceArgumentFilterParameter(parameterSyntax.Identifier.Text, parameterSymbol.Type.GetFullyQualifiedTypeName(), parameterSymbol.IsServiceRequired());
            }
        }

        bool isRef = parameterSyntax.Modifiers.Any(x => x.IsKind(SyntaxKind.RefKeyword));
        return isRef 
            ? new RefValueArgumentFilterParameter(parameterSyntax.Identifier.Text, parameterSymbol.Type!, parameterSymbol.IsParameterRequired())
            : new ValueArgumentFilterParameter(parameterSyntax.Identifier.Text, parameterSymbol.Type!, parameterSymbol.IsParameterRequired());
    }
}
