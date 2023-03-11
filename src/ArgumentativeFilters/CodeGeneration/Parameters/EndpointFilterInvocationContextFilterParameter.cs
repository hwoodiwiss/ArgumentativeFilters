namespace ArgumentativeFilters.CodeGeneration.Parameters;

public class EndpointFilterInvocationContextFilterParameter : ArgumentativeFilterParameterProvider
{
    public override string ParameterCode => VariableNames.InvocationFilterContext;
}