using System.Threading.Tasks;

using ArgumentativeFilters;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ExampleMinimalApi.Filters;

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
        [FromServices] ValidateIdFilter filterInstance)
    {
        return filterInstance.ValidateId(context, next, ref id);
    }
}