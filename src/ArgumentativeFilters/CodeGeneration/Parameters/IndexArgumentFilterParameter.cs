using ArgumentativeFilters.CodeGeneration.Parameters.Abstract;

namespace ArgumentativeFilters.CodeGeneration.Parameters;

public class IndexArgumentFilterParameter : ArgumentativeFilterParameterProvider, IFactoryCodeProvider, IFilterConditionProvider 
{
    protected readonly string _argumentName;
    private const string NameSuffix = "Index";
    
    public IndexArgumentFilterParameter(string argumentName)
    {
        _argumentName = argumentName;
    }
    
        
    public string FactoryCode => $"int? {_argumentName}{NameSuffix} = context.GetArgumentIndex(\"{_argumentName}\");";
    public string FilterConditionCode => $"{_argumentName}{NameSuffix}.HasValue";
    public override string ParameterCode => $"{_argumentName}{NameSuffix}.Value";
}