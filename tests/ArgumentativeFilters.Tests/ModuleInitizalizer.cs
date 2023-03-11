using System.Runtime.CompilerServices;

namespace ArgumentativeFilters.Tests;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifySourceGenerators.Enable();
    }
}