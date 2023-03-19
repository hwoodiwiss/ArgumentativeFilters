using ArgumentativeFilters.CodeGeneration.Parameters.Abstract;

namespace ArgumentativeFilters.CodeGeneration.Parameters;

public sealed class ValueArgumentFilterParameter : IndexArgumentFilterParameter, IFilterCodeProvider
{
    private readonly string _argumentType;
    private const string ValueNameSuffix = "Value";
    
    public ValueArgumentFilterParameter(string argumentName, string argumentType) : base(argumentName)
    {
        _argumentType = argumentType;
    }

    public string FilterCode => $"{_argumentType} {_argumentName}{ValueNameSuffix} = {VariableNames.InvocationFilterContext}.GetArgument<{_argumentType}>({_argumentName}{IndexNameSuffix}.Value);";

    public override string ParameterCode => $"{_argumentName}{ValueNameSuffix}";
}