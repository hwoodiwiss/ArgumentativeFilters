using ArgumentativeFilters.CodeGeneration.Parameters.Abstract;

namespace ArgumentativeFilters.CodeGeneration.Parameters;

public sealed class ValueArgumentFilterParameter : IndexArgumentFilterParameter, IFilterCodeProvider, IFactoryCodeProvider, IFilterConditionProvider
{
    private readonly string _argumentType;
    private const string IndexNameSuffix = "ValueIndex";
    private const string ValueNameSuffix = "Value";
    
    public ValueArgumentFilterParameter(string argumentName, string argumentType) : base(argumentName)
    {
        _argumentType = argumentType;
    }

    public string FilterCode => $"{_argumentType} {_argumentName}{ValueNameSuffix} = {VariableNames.InvocationFilterContext}.GetArgument<{_argumentType}>({_argumentName}{IndexNameSuffix}.Value);";
    public new string FilterConditionCode => $"{_argumentName}{IndexNameSuffix}.HasValue";

    public override string ParameterCode => $"{_argumentName}{ValueNameSuffix}";
    public new string FactoryCode => $"int? {_argumentName}{IndexNameSuffix} = context.GetArgumentIndex(\"{_argumentName}\");";
}