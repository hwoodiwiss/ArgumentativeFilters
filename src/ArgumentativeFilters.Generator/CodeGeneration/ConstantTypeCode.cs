namespace ArgumentativeFilters.Generator.CodeGeneration;

public static class ConstantTypeCode
{
    public static string ArgumentativeFiltersParameterHelpers => """
namespace ArgumentativeFilters
{
    /// <summary>
    /// A static class containing helper utilities used by ArgumentativeFilters
    /// </summary>
    public static class ArgumentativeFiltersParameterHelpers
    {
        /// <summary>
        /// A static helper method for finding the index of a named parameter on a MinimalAPI endpoint, given an <see cref="global::Microsoft.AspNetCore.Http.EndpointFilterFactoryContext" />.
        /// </summary>
        /// <param name="context">The <see cref="global::Microsoft.AspNetCore.Http.EndpointFilterFactoryContext" /> within which to search for the named parameter.</param>
        /// <param name="argumentName">The argument name to search for within the <see cref="global::Microsoft.AspNetCore.Http.EndpointFilterFactoryContext" />.</param>
        /// <param name="comparisonMode">The string comparison mode to use when searching for parameter indices.</param>
        /// <returns>
        /// The endpoint delegate parameter index for the given argument name as a <see cref="int?" />.
        /// Returns <see langword="null"/> if the argument name is not found.
        /// </returns>
        public static int? GetArgumentIndex(global::Microsoft.AspNetCore.Http.EndpointFilterFactoryContext context, string argumentName, StringComparison comparisonMode)
            => context.MethodInfo.GetParameters().FirstOrDefault(p => string.Equals(p.Name, argumentName, comparisonMode))?.Position;
    }
}  
""";
}