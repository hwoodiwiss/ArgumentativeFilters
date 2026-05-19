namespace ArgumentativeFilters;

/// <summary>
/// A method marked with this attribute will have argumentative middleware generated for it.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public sealed class ArgumentativeMiddlewareAttribute : Attribute
{
    /// <summary>
    /// If set, the generated middleware method name will be prefixed with this value.
    /// </summary>
    public string? Prefix { get; set; }
}
