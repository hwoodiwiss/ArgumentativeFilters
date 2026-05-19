using System.Collections.Immutable;
using System.Diagnostics;
using System.Text;

using ArgumentativeFilters.Generator.CodeGeneration;
using ArgumentativeFilters.Generator.CodeGeneration.Extensions;
using ArgumentativeFilters.Generator.Extensions;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace ArgumentativeFilters.Generator;

[Generator]
public class ArgumentativeMiddlewareGenerator : IIncrementalGenerator
{
    const string ArgumentativeMiddlewareAttributeName = "ArgumentativeFilters.ArgumentativeMiddlewareAttribute";
    const string AspNetCoreHttpContextName = "Microsoft.AspNetCore.Http.HttpContext";
    const string AspNetCoreRequestDelegateName = "Microsoft.AspNetCore.Http.RequestDelegate";
    const string FromKeyedServicesAttributeName = "Microsoft.Extensions.DependencyInjection.FromKeyedServicesAttribute";
    const string ServiceVariableSuffix = "Service";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValuesProvider<MethodDeclarationSyntax?> methodDeclarationSyntax = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                ArgumentativeMiddlewareAttributeName,
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

                if (attributeSymbol.ContainingType.ToDisplayString() == ArgumentativeMiddlewareAttributeName)
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

        foreach (var middleware in methods)
        {
            GenerateMiddleware(compilation, middleware, sb);
            sb.AppendLine();
        }

        var sourceText = SourceText.From(sb.ToString(), Encoding.UTF8);
        if (debugOutputEnabled)
        {
            var elapsed = codegenTimer.Elapsed;
            sourceText = sourceText.WithChanges(new List<TextChange>(1)
            {
                new(TextSpan.FromBounds(sourceText.Length, sourceText.Length),
                    $"\n// Generated middleware in {elapsed.TotalMilliseconds}ms.\n")
            });
        }

        context.AddSource("ArgumentativeMiddleware.g.cs", sourceText);
    }

    private static void GenerateMiddleware(Compilation compilation, MethodDeclarationSyntax middleware, StringBuilder sb)
    {
        var semanticModel = compilation.GetSemanticModel(middleware.SyntaxTree);
        var middlewareMethodSymbol = semanticModel.GetDeclaredSymbol(middleware);

        if (middlewareMethodSymbol is null)
        {
            return;
        }

        if (!IsSupportedReturnType(middlewareMethodSymbol.ReturnType, out bool returnsValueTask))
        {
            return;
        }

        var middlewareNamePrefix = middlewareMethodSymbol.GetAttributeNamedParameterValue(ArgumentativeMiddlewareAttributeName, "Prefix");
        var containingNamespace = middlewareMethodSymbol.ContainingNamespace.ToDisplayString();

        var containingClassSyntax = middleware.Parent switch
        {
            ClassDeclarationSyntax cds => cds,
            _ => throw new InvalidOperationException("Middleware must be declared in a partial class.")
        };

        if (containingClassSyntax.Modifiers.All(a => !a.IsKind(SyntaxKind.PartialKeyword)))
        {
            return;
        }

        List<string> middlewareCode = [];
        List<string> parameterCode = [];
        foreach (var parameter in middlewareMethodSymbol.Parameters)
        {
            if (!TryGetMiddlewareParameterCode(parameter, out string? generatedCode, out string generatedParameter))
            {
                return;
            }

            if (generatedCode is not null)
            {
                middlewareCode.Add(generatedCode);
            }

            parameterCode.Add(generatedParameter);
        }

        var containingClass = middlewareMethodSymbol.ContainingType!;
        var containingTypes = GetContainingTypes(middlewareMethodSymbol);

        FilterFactoryContainingHierarchyBuilder hierarchyBuilder = new(sb);

        hierarchyBuilder.AddContainingHierarchy(containingTypes, containingNamespace);

        MiddlewareBuilder builder = new(sb, hierarchyBuilder.CurrentIndentationLevel);
        builder
            .AddGeneratedCodeAttribute()
            .AddMiddlewareSignature(containingClass.GetAccessibilityString(), middlewareNamePrefix)
            .StartMiddlewareClosure();

        foreach (var code in middlewareCode)
        {
            builder.AddMiddlewareCode(code);
        }

        builder
            .AddMiddlewareCall(middleware.Identifier.Text, parameterCode.ToImmutableArray(), returnsValueTask)
            .EndMiddlewareClosure()
            .EndMiddleware();

        hierarchyBuilder.CloseContainingHierarchy();
    }

    private static bool TryGetMiddlewareParameterCode(IParameterSymbol parameterSymbol, out string? middlewareCode, out string parameterCode)
    {
        middlewareCode = null;

        if (parameterSymbol.Type?.ToDisplayString() == AspNetCoreHttpContextName)
        {
            parameterCode = VariableNames.InvocationHttpContext;
            return true;
        }

        if (parameterSymbol.Type?.ToDisplayString() == AspNetCoreRequestDelegateName)
        {
            parameterCode = VariableNames.RequestDelegate;
            return true;
        }

        if (parameterSymbol.RefKind != RefKind.None)
        {
            parameterCode = string.Empty;
            return false;
        }

        string? key = parameterSymbol
            .GetAttributes()
            .FirstOrDefault(attribute => attribute.AttributeClass?.ToDisplayString() == FromKeyedServicesAttributeName)?
            .ConstructorArguments
            .FirstOrDefault()
            .ToCSharpString();

        string parameterType = parameterSymbol.Type.GetFullyQualifiedTypeName();
        string resolvedServiceName = $"{parameterSymbol.Name}{ServiceVariableSuffix}";
        bool required = parameterSymbol.IsServiceRequired();
        middlewareCode = $"{parameterType}{(required ? string.Empty : "?")} {resolvedServiceName} = {VariableNames.InvocationHttpContext}.RequestServices.{GetServiceProviderMethodName(required, key is not null)}<{parameterType}>({GetServiceLookupArgument(key)});";
        parameterCode = resolvedServiceName;
        return true;
    }

    private static bool IsSupportedReturnType(ITypeSymbol returnType, out bool returnsValueTask)
    {
        string returnTypeName = returnType.ToDisplayString();

        if (returnTypeName == "System.Threading.Tasks.Task")
        {
            returnsValueTask = false;
            return true;
        }

        if (returnTypeName == "System.Threading.Tasks.ValueTask")
        {
            returnsValueTask = true;
            return true;
        }

        returnsValueTask = false;
        return false;
    }

    private static string GetServiceLookupArgument(string? key)
        => key is null ? string.Empty : key;

    private static string GetServiceProviderMethodName(bool required, bool keyed)
        => $"Get{ValueOrEmpty(required, "Required")}{ValueOrEmpty(keyed, "Keyed")}Service";

    private static string ValueOrEmpty(bool condition, string value)
        => condition ? value : string.Empty;

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
