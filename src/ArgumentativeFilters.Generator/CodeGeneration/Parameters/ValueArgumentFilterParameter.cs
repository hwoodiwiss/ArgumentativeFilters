using ArgumentativeFilters.Generator.CodeGeneration.Parameters.Abstract;
using ArgumentativeFilters.Generator.Extensions;

namespace ArgumentativeFilters.Generator.CodeGeneration.Parameters;

public sealed class ValueArgumentFilterParameter : IndexArgumentFilterParameter, IFilterCodeProvider
{
    private readonly ITypeSymbol _argumentType;
    private const string ValueNameSuffix = "Value";

    public ValueArgumentFilterParameter(string argumentName, ITypeSymbol argumentType, bool required)
        : base(argumentName, required)
    {
        _argumentType = argumentType;
    }

    public string FilterCode
    {
        get
        {
            string typeText = _argumentType.GetFullyQualifiedTypeName();
            string nullableAnnotation = _argumentType.IsReferenceType ? "?" : string.Empty;

            return _required
                ? $"{typeText} {_argumentName}{ValueNameSuffix} = {VariableNames.InvocationFilterContext}.GetArgument<{typeText}>({_argumentName}{IndexNameSuffix}.Value);"
                : $"{typeText}{nullableAnnotation} {_argumentName}{ValueNameSuffix} = {_argumentName}{IndexNameSuffix}.HasValue ? {VariableNames.InvocationFilterContext}.GetArgument<{typeText}>({_argumentName}{IndexNameSuffix}.Value) : null;";
        }
    }

    public override string ParameterCode => $"{_argumentName}{ValueNameSuffix}";
}