using ArgumentativeFilters.CodeGeneration.Parameters.Abstract;

namespace ArgumentativeFilters.CodeGeneration.Parameters;

public sealed class ValueArgumentFilterParameter : IndexArgumentFilterParameter, IFilterCodeProvider
{
    private readonly string _argumentType;
    private const string ValueNameSuffix = "Value";
    
    public ValueArgumentFilterParameter(string argumentName, string argumentType, bool required) 
        : base(argumentName, required)
    {
        _argumentType = argumentType;
    }

    public string FilterCode => _required ? $"{_argumentType} {_argumentName}{ValueNameSuffix} = {VariableNames.InvocationFilterContext}.GetArgument<{_argumentType}>({_argumentName}{IndexNameSuffix}.Value);"
        : $"{_argumentType}? {_argumentName}{ValueNameSuffix} = {_argumentName}{IndexNameSuffix}.HasValue ? {VariableNames.InvocationFilterContext}.GetArgument<{_argumentType}>({_argumentName}{IndexNameSuffix}.Value) : null;";

    public override string ParameterCode => $"{_argumentName}{ValueNameSuffix}";
}