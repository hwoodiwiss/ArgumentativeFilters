using ArgumentativeFilters.Generator;
using BenchmarkDotNet.Attributes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace ArgumentativeFilters.Benchmarks;

[MemoryDiagnoser]
public class GeneratorBenchmarks
{
    private CSharpCompilation _filterCompilation = null!;
    private CSharpCompilation _middlewareCompilation = null!;
    private ArgumentativeFilterFactoryGenerator _filterGenerator = null!;
    private ArgumentativeMiddlewareGenerator _middlewareGenerator = null!;

    [GlobalSetup]
    public void Setup()
    {
        var references = GetMetadataReferences();

        _filterGenerator = new ArgumentativeFilterFactoryGenerator();
        _middlewareGenerator = new ArgumentativeMiddlewareGenerator();

        _filterCompilation = CreateCompilation(
            """
            using ArgumentativeFilters;
            using Microsoft.AspNetCore.Http;

            namespace BenchmarkTest;

            public static partial class TestFilter
            {
                [ArgumentativeFilter]
                public static ValueTask<object?> Filter(
                    EndpointFilterInvocationContext context,
                    EndpointFilterDelegate next,
                    int id) => next(context);
            }
            """,
            references);

        _middlewareCompilation = CreateCompilation(
            """
            using ArgumentativeFilters;
            using Microsoft.AspNetCore.Http;

            namespace BenchmarkTest;

            public static partial class TestMiddleware
            {
                [ArgumentativeMiddleware]
                public static Task Middleware(HttpContext context, RequestDelegate next)
                    => next(context);
            }
            """,
            references);
    }

    /// <summary>
    /// Benchmarks running the ArgumentativeFilter source generator on a compilation containing
    /// a single method annotated with <c>[ArgumentativeFilter]</c>.
    /// </summary>
    [Benchmark]
    public GeneratorDriverRunResult FilterGeneration()
    {
        var driver = CSharpGeneratorDriver.Create(_filterGenerator);
        return driver.RunGenerators(_filterCompilation).GetRunResult();
    }

    /// <summary>
    /// Benchmarks running the ArgumentativeMiddleware source generator on a compilation containing
    /// a single method annotated with <c>[ArgumentativeMiddleware]</c>.
    /// </summary>
    [Benchmark]
    public GeneratorDriverRunResult MiddlewareGeneration()
    {
        var driver = CSharpGeneratorDriver.Create(_middlewareGenerator);
        return driver.RunGenerators(_middlewareCompilation).GetRunResult();
    }

    private static CSharpCompilation CreateCompilation(string source, IEnumerable<MetadataReference> references)
        => CSharpCompilation.Create(
            "BenchmarkAssembly",
            [CSharpSyntaxTree.ParseText(source)],
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

    private static IEnumerable<MetadataReference> GetMetadataReferences()
    {
        var trustedAssemblies = ((string)(AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") ?? string.Empty))
            .Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries);

        return trustedAssemblies
            .Select(path => (MetadataReference)MetadataReference.CreateFromFile(path))
            .Append(MetadataReference.CreateFromFile(typeof(ArgumentativeFilterAttribute).Assembly.Location));
    }
}
