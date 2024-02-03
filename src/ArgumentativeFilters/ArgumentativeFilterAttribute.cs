
namespace ArgumentativeFilters;

/// <summary>
/// A method marked with this attribute will have an argumentative filters factory method generated for it.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public sealed class ArgumentativeFilterAttribute : Attribute
{
    /// <summary>
    /// If set, the generated factory method name be prefixed with this value.
    /// </summary>
    public string? Prefix { get; set; }
}
