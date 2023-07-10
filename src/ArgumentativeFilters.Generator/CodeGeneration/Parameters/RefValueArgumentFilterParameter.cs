using ArgumentativeFilters.Generator.CodeGeneration.Parameters.Abstract;
using ArgumentativeFilters.Generator.Extensions;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ArgumentativeFilters.Generator.CodeGeneration.Parameters;

public sealed class RefValueArgumentFilterParameter : ValueArgumentFilterParameter
{
    private const string ValueNameSuffix = "Value";

    public RefValueArgumentFilterParameter(string argumentName, ITypeSymbol argumentType, bool required)
        : base(argumentName, argumentType, required)
    {
    }

    public override string ParameterCode => $"ref {_argumentName}{ValueNameSuffix}";
}