using ArgumentativeFilters.CodeGeneration.Parameters.Abstract;

namespace ArgumentativeFilters.CodeGeneration.Parameters;

public class IndexArgumentFilterParameter : ArgumentativeFilterParameterProvider, IFactoryCodeProvider, IFilterConditionProvider 
{
    protected readonly string _argumentName;
    protected const string IndexNameSuffix = "Index";
    
    public IndexArgumentFilterParameter(string argumentName)
    {
        _argumentName = argumentName;
    }
    
        
    public string FactoryCode => $"int? {_argumentName}{IndexNameSuffix} = context.GetArgumentIndex(\"{_argumentName}\");";
    public string FilterConditionCode => $"{_argumentName}{IndexNameSuffix}.HasValue";
    public override string ParameterCode => $"{_argumentName}{IndexNameSuffix}.Value";
}