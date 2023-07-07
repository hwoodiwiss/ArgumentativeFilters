namespace ArgumentativeFilters;

/// <summary>
/// A method marked with this attribute will contain the index of the provided argument name.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
public sealed class IndexOfAttribute : Attribute
{
    public IndexOfAttribute(string argumentName)
    {
        {
            ArgumentName = argumentName;
        }
    }

    public string ArgumentName { get; }
}