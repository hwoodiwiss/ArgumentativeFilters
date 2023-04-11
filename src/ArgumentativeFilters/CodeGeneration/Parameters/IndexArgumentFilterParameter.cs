using ArgumentativeFilters.CodeGeneration.Parameters.Abstract;

namespace ArgumentativeFilters.CodeGeneration.Parameters;

public class IndexArgumentFilterParameter : ArgumentativeFilterParameterProvider, IFactoryCodeProvider, IFilterConditionProvider 
{
    protected readonly string _argumentName;
    protected readonly bool _required;
    protected const string IndexNameSuffix = "Index";
    
    public IndexArgumentFilterParameter(string argumentName, bool required)
    {
        _argumentName = argumentName;
        _required = required;
    }
    
        
    public string FactoryCode => $"int? {_argumentName}{IndexNameSuffix} = GetArgumentIndex({VariableNames.FactoryFilterContext}, \"{_argumentName}\");";
    
    public string FilterConditionCode => _required ? $"{_argumentName}{IndexNameSuffix}.HasValue" : string.Empty;
    
    public override string ParameterCode => _required ? $"{_argumentName}{IndexNameSuffix}.Value" : $"{_argumentName}{IndexNameSuffix}";
}