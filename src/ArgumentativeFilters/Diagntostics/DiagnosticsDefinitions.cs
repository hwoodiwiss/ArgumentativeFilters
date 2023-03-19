namespace ArgumentativeFilters;

public class Diagnostics
{
    public static readonly DiagnosticDescriptor NonPartialClassDiagnostic = new DiagnosticDescriptor("ARGF001",
        "Argumentative filter non-partial parent",
        "The class {0} is not partial",
        "Usage",
        DiagnosticSeverity.Error,
        true);
}