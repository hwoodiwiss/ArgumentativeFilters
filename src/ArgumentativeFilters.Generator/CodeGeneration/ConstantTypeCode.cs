namespace ArgumentativeFilters.Generator.CodeGeneration;

public static class ConstantTypeCode
{
    public static string ArgumentativeFiltersParameterHelpers => $$"""
namespace ArgumentativeFilters
{
    /// <summary>
    /// A static class containing helper utilities used by ArgumentativeFilters
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{AssemblyMetadata.Name}}", "{{AssemblyMetadata.Version}}")]
    public static class ArgumentativeFiltersParameterHelpers
    {
        /// <summary>
        /// A static helper method for finding the index of a named parameter on a MinimalAPI endpoint, given an <see cref="global::Microsoft.AspNetCore.Http.EndpointFilterFactoryContext" />.
        /// </summary>
        /// <param name="context">The <see cref="global::Microsoft.AspNetCore.Http.EndpointFilterFactoryContext" /> to search for the provided argument names index in.</param>
        /// <param name="argumentName">The argument name to search for in the <see cref="global::Microsoft.AspNetCore.Http.EndpointFilterFactoryContext" />'s <c>MethodInfo</c> property.</param>
        /// <param name="comparisonMode">The string comparison mode to use when searching for argument indices.</param>
        /// <returns>
        /// The endpoint delegate parameter index for the given argument name as a <see cref="System.Nullable{T}" /> of type <see cref="int" />.
        /// Returns <see langword="null"/> if the argument name is not found.
        /// </returns>
        public static int? GetArgumentIndex(global::Microsoft.AspNetCore.Http.EndpointFilterFactoryContext context, string argumentName, StringComparison comparisonMode)
            => context.MethodInfo.GetParameters().FirstOrDefault(p => string.Equals(p.Name, argumentName, comparisonMode))?.Position;
    }
}  
""";
}