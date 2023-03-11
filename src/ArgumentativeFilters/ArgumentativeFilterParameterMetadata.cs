using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ArgumentativeFilters;

public class FilterParameterMetadata
{
    protected readonly ParameterSyntax _parameter;
    protected readonly SemanticModel _semanticModel;
    
    public FilterParameterMetadata(ParameterSyntax parameter, SemanticModel semanticModel)
    {
        _parameter = parameter;
        _semanticModel = semanticModel;
    }
    
};

public class IndexOfFilterParameterMetadata : FilterParameterMetadata
{

    public IndexOfFilterParameterMetadata(ParameterSyntax parameter, SemanticModel semanticModel) 
        : base(parameter, semanticModel)
    {
        foreach(var attributeList in parameter.AttributeLists)
        {
            foreach(var attribute in attributeList.Attributes)
            {
                if (semanticModel.GetSymbolInfo(attribute).Symbol is not IMethodSymbol attributeSymbol)
                {
                    continue;
                }
                
                INamedTypeSymbol attributeContainingTypeSymbol = attributeSymbol.ContainingType;
                string fullName = attributeContainingTypeSymbol.ToDisplayString();

                if (fullName == "ArgumentativeFilters.IndexOfAttribute")
                {
                        
                }
                
            }
        }
    }
    
};