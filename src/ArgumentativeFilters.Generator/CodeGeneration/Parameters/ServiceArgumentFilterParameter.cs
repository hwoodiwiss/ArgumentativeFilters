using ArgumentativeFilters.Generator.CodeGeneration.Parameters.Abstract;

namespace ArgumentativeFilters.Generator.CodeGeneration.Parameters;

public class ServiceArgumentFilterParameter : ArgumentativeFilterParameterProvider, IFilterCodeProvider
{
    private readonly string _argumentName;
    private readonly string _argumentType;
    private readonly bool _required;
    private readonly string? _key;
    private const string NameSuffix = "Service";

    public ServiceArgumentFilterParameter(string argumentName, string argumentType, bool required, string? key)
    {
        _argumentName = argumentName;
        _argumentType = argumentType;
        _required = required;
        _key = key;
    }

    public string FilterCode => $"{_argumentType}{(_required ? string.Empty : "?")} {_argumentName}{NameSuffix} = {VariableNames.InvocationFilterContext}.HttpContext.RequestServices.{GetServiceProviderMethodName()}<{_argumentType}>({_key});";

    public override string ParameterCode => $"{_argumentName}{NameSuffix}";

    private string GetServiceProviderMethodName() 
        => $"Get{ValueOrEmpty(_required, "Required")}{ValueOrEmpty(_key is not null, "Keyed")}Service";
    
    private static string ValueOrEmpty(bool condition, string value) 
        => condition ? value : string.Empty;
}