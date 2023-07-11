using ArgumentativeFilters.Generator.CodeGeneration.Parameters.Abstract;
using ArgumentativeFilters.Generator.Extensions;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ArgumentativeFilters.Generator.CodeGeneration.Parameters;

public sealed class RefValueArgumentFilterParameter : ValueArgumentFilterParameter
{
    private readonly RefKind _refKind;
    private const string ValueNameSuffix = "Value";

    public RefValueArgumentFilterParameter(RefKind refKind, string argumentName, ITypeSymbol argumentType, bool required)
        : base(argumentName, argumentType, required)
    {
        _refKind = refKind;
    }

    public override string ParameterCode => $"{GetRefParameterPrefix()}{_argumentName}{ValueNameSuffix}";
    
    private string GetRefParameterPrefix() 
        => _refKind switch
        {
            RefKind.Ref => "ref ",
            RefKind.In => "in ",
            _ => string.Empty
        };
}