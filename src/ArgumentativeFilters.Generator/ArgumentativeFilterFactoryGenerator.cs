using System.Collections.Immutable;
using System.Diagnostics;
using System.Text;

using ArgumentativeFilters.Generator.CodeGeneration;
using ArgumentativeFilters.Generator.CodeGeneration.Extensions;
using ArgumentativeFilters.Generator.CodeGeneration.Parameters.Abstract;
using ArgumentativeFilters.Generator.Extensions;
using ArgumentativeFilters.Generator.Parsing;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace ArgumentativeFilters.Generator;

[Generator]
public class ArgumentativeFilterFactoryGenerator : IIncrementalGenerator
{
    const string ArgumentativeFiltersAttributeName = "ArgumentativeFilters.ArgumentativeFilterAttribute";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValuesProvider<MethodDeclarationSyntax?> methodDeclarationSyntax = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                ArgumentativeFiltersAttributeName,
                predicate: static (s, _) => IsSyntaxTargetForGeneration(s),
                transform: static (ctx, _) => GetSemanticTargetForGeneration(ctx))
            .Where(static m => m is not null);

        IncrementalValueProvider<(AnalyzerConfigOptionsProvider OptionsProvider, (Compilation Compilation, ImmutableArray<MethodDeclarationSyntax?> MethodDeclarations) Syntax)> methodDeclarationsAndAnalyzerConfigOptions
            = context.AnalyzerConfigOptionsProvider.Combine(context.CompilationProvider.Combine(methodDeclarationSyntax.Collect()));

        context.RegisterSourceOutput(methodDeclarationsAndAnalyzerConfigOptions,
            static (spc, source) => Execute(source.Syntax.Compilation, source.Syntax.MethodDeclarations!, spc, source.OptionsProvider));
    }

    private static bool IsSyntaxTargetForGeneration(SyntaxNode syntaxNode) =>
        syntaxNode is MethodDeclarationSyntax { AttributeLists.Count: > 0 };

    private static MethodDeclarationSyntax? GetSemanticTargetForGeneration(GeneratorAttributeSyntaxContext context)
    {
        var methodDeclarationSyntax = context.TargetNode as MethodDeclarationSyntax;

        foreach (var attributeList in methodDeclarationSyntax!.AttributeLists)
        {
            foreach (var attribute in attributeList.Attributes)
            {
                if (context.SemanticModel.GetSymbolInfo(attribute).Symbol is not IMethodSymbol attributeSymbol)
                {
                    continue;
                }

                INamedTypeSymbol attributeContainingTypeSymbol = attributeSymbol.ContainingType;
                string fullName = attributeContainingTypeSymbol.ToDisplayString();

                if (fullName == ArgumentativeFiltersAttributeName)
                {
                    return methodDeclarationSyntax;
                }

            }
        }

        return null;
    }

    static void Execute(Compilation compilation, ImmutableArray<MethodDeclarationSyntax> methods, SourceProductionContext context, AnalyzerConfigOptionsProvider optionsProvider)
    {
        if (methods.IsDefaultOrEmpty)
        {
            return;
        }

        bool debugOutputEnabled = optionsProvider.GlobalOptions.TryGetValue("build_property.ArgumentativeFilters_WriteDebug", out string? _);

        StringBuilder sb = new();

        sb.Append(CodeSnippets.ArgumentativeFilterFileHeader);
        Stopwatch codegenTimer = new();

        if (debugOutputEnabled)
        {
            codegenTimer.Start();
        }

        foreach (var filter in methods)
        {
            GenerateFilterFactory(compilation, filter, sb);
            sb.AppendLine();
        }

        sb.AppendLine(ConstantTypeCode.ArgumentativeFiltersParameterHelpers);

        var sourceText = SourceText.From(sb.ToString(), Encoding.UTF8);
        if (debugOutputEnabled)
        {
            var elapsed = codegenTimer.Elapsed;
            sourceText = sourceText.WithChanges(new List<TextChange>(1)
            {
                new(TextSpan.FromBounds(sourceText.Length, sourceText.Length),
                    $"\n// Generated filter factories in {elapsed.TotalMilliseconds}ms.\n")
            });
        }

        context.AddSource($"ArgumentativeFilters.g.cs", sourceText);
    }

    private static void GenerateFilterFactory(Compilation compilation, MethodDeclarationSyntax filter, StringBuilder sb)
    {
        var semanticModel = compilation.GetSemanticModel(filter.SyntaxTree);
        var filterMethodSymbol = semanticModel.GetDeclaredSymbol(filter);
        
        if (filterMethodSymbol is null)
        {
            return;
        }

        var factoryNamePrefix = filterMethodSymbol.GetAttributeNamedParameterValue(ArgumentativeFiltersAttributeName, "Prefix");
        
        var containingNamespace = filterMethodSymbol.ContainingNamespace.ToDisplayString();

        var containingClassSyntax = filter.Parent switch
        {
            ClassDeclarationSyntax cds => cds,
            _ => throw new InvalidOperationException("Filter must be declared in a partial class.")
        };

        if (containingClassSyntax.Modifiers.All(a => !a.IsKind(SyntaxKind.PartialKeyword)))
        {
            return;
        }

        var containingClass = filterMethodSymbol.ContainingType!;
        var parameters = filter.ParameterList.Parameters.Select(s => ParameterCodeProviderFactory.GetParameterCodeProvider(s, compilation)).ToArray();
        var containingTypes = GetContainingTypes(filterMethodSymbol);


        FilterFactoryContainingHierarchyBuilder hierarchyBuilder = new(sb);

        hierarchyBuilder.AddContainingHierarchy(containingTypes, containingNamespace);

        FilterFactoryBuilder builder = new(sb, hierarchyBuilder.CurrentIndentationLevel);
        builder
            .AddGeneratedCodeAttribute()
            .AddFilterFactorySignature(containingClass.GetAccessibilityString(), factoryNamePrefix)
            .AddFactoryCode(parameters.OfType<IFactoryCodeProvider>().ToImmutableArray())
            .AddFilterConditionCode(parameters.OfType<IFilterConditionProvider>().ToImmutableArray())
            .StartFilterClosure()
            .AddFilterCode(parameters.OfType<IFilterCodeProvider>().ToImmutableArray())
            .AddFilterCall(filter.Identifier.Text, parameters.ToImmutableArray())
            .EndFilterClosure()
            .EndFilterCondition()
            .EndFilterFactory();

        hierarchyBuilder.CloseContainingHierarchy();
    }

    private static Stack<INamedTypeSymbol> GetContainingTypes(IMethodSymbol typeSymbol)
    {
        var stack = new Stack<INamedTypeSymbol>();
        var current = typeSymbol.ContainingType;
        while (current is not null)
        {
            stack.Push(current);
            current = current.ContainingType;
        }

        return stack;
    }
}