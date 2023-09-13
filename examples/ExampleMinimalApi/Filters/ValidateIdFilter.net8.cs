using System.Threading.Tasks;

using ArgumentativeFilters;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ExampleMinimalApi.Filters;

public enum ValidateIdFilters
{
    ValidateId
}

public sealed partial class ValidateIdFilter
{
    private readonly ExampleMinimalApiOptions _options;

    public ValidateIdFilter(IOptionsSnapshot<ExampleMinimalApiOptions> options)
    {
        _options = options.Value;
    }
    
    public ValueTask<object?> ValidateId(EndpointFilterInvocationContext context, EndpointFilterDelegate next, ref int id) 
    {
        if (id < _options.MinimumAllowedId)
        {
            return ValueTask.FromResult<object?>(Results.BadRequest($"id must be {_options.MinimumAllowedId} or greater"));
        }

        return next(context);
    }
    
    [ArgumentativeFilter]
    private static ValueTask<object?> Filter(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next,
        ref int id,
        [FromKeyedServices(ValidateIdFilters.ValidateId)] ValidateIdFilter filterInstance)
    {
        return filterInstance.ValidateId(context, next, ref id);
    }
}