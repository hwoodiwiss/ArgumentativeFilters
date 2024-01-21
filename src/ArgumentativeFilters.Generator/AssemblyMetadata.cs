using System.Reflection;

namespace ArgumentativeFilters.Generator;

public static class AssemblyMetadata
{
    public static string Name { get; } = typeof(AssemblyMetadata).Assembly.GetName().Name!;

    public static string? Version { get; } = GetAssemblyVersion();

    private static string? GetAssemblyVersion()
    {
        var assembly = typeof(AssemblyMetadata).Assembly;
        var assemblyVersion = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
        return assemblyVersion;
    }
}