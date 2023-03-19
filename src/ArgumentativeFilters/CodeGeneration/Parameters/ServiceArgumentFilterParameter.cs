using ArgumentativeFilters.CodeGeneration.Parameters.Abstract;

namespace ArgumentativeFilters.CodeGeneration.Parameters;

public class ServiceArgumentFilterParameter : ArgumentativeFilterParameterProvider, IFilterCodeProvider
{
    private readonly string _argumentName;
    private readonly string _argumentType;
    private readonly bool _required;
    private const string NameSuffix = "Service";
    private const string GetRequiredServiceMethodName = "GetRequiredService";
    private const string GetServiceMethodName = "GetService";
    
    public ServiceArgumentFilterParameter(string argumentName, string argumentType, bool required)
    {
        _argumentName = argumentName;
        _argumentType = argumentType;
        _required = required;
    }

    public string FilterCode => $"var {_argumentName}{NameSuffix} = {VariableNames.InvocationFilterContext}.HttpContext.RequestServices.{GetServiceProviderMethodName()}<{_argumentType}>();";
    public override string ParameterCode => $"{_argumentName}{NameSuffix}";

    private string GetServiceProviderMethodName() => _required 
        ? GetRequiredServiceMethodName 
        : GetServiceMethodName;
}