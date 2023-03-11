using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace ArgumentativeFilters.Tests;

public static class TestHelper
{
    public static Task Verify(string source)
    {
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(source);
        
        IEnumerable<PortableExecutableReference> references = new[]
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(EndpointFilterInvocationContext).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(MulticastDelegate).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(IList<>).Assembly.Location),
        };

        var compilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);

        CSharpCompilation compilation = CSharpCompilation.Create(
            assemblyName: "Tests",
            syntaxTrees: new[] { syntaxTree },
            options: compilationOptions,
            references: references);

        var generator = new ArgumentativeFilterFactoryGenerator();

        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

        driver = driver.RunGenerators(compilation);

        var a = driver.GetRunResult().Results.SelectMany(s => s.GeneratedSources);
        
        return Verifier.Verify(driver);
    }
}