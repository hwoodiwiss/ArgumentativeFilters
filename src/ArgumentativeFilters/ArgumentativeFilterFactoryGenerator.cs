using System.Collections.Immutable;
using System.Text;

using ArgumentativeFilters.CodeGeneration;
using ArgumentativeFilters.CodeGeneration.Parameters.Abstract;
using ArgumentativeFilters.Parsing;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace ArgumentativeFilters;

[Generator]
public class ArgumentativeFilterFactoryGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
            "ArgumentativeFilterAttribute.g.cs", 
            SourceText.From(TypeTemplates.ArgumentativeFilterAttribute, Encoding.UTF8)));
        
        context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
            "IndexOfArgumentAttribute.g.cs", 
            SourceText.From(TypeTemplates.IndexOfArgumentAttribute, Encoding.UTF8)));

        IncrementalValuesProvider<MethodDeclarationSyntax?> methodDeclarationSyntax = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (s, _) => IsSyntaxTargetForGeneration(s),
                transform: static (ctx, _) => GetSemanticTargetForGeneration(ctx))
            .Where(static m => m is not null);
        
        // Combine the selected enums with the `Compilation`
        IncrementalValueProvider<(Compilation, ImmutableArray<MethodDeclarationSyntax?>)> compilationAndEnums
            = context.CompilationProvider.Combine(methodDeclarationSyntax.Collect());
        
        context.RegisterSourceOutput(compilationAndEnums,
            static (spc, source) => Execute(source.Item1, source.Item2!, spc));
    }
    
    private static bool IsSyntaxTargetForGeneration(SyntaxNode syntaxNode) =>
        syntaxNode is MethodDeclarationSyntax { AttributeLists.Count: > 0 };

    private static MethodDeclarationSyntax? GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
    {
        var methodDeclarationSyntax = context.Node as MethodDeclarationSyntax;
        
        foreach(var attributeList in methodDeclarationSyntax!.AttributeLists)
        {
            foreach(var attribute in attributeList.Attributes)
            {
                if (ModelExtensions.GetSymbolInfo(context.SemanticModel, attribute).Symbol is not IMethodSymbol attributeSymbol)
                {
                    continue;
                }
                
                INamedTypeSymbol attributeContainingTypeSymbol = attributeSymbol.ContainingType;
                string fullName = attributeContainingTypeSymbol.ToDisplayString();
                
                if (fullName == "ArgumentativeFilters.ArgumentativeFilterAttribute")
                {
                    return methodDeclarationSyntax;
                }
                
            }
        }

        return null;
    }
        
    static void Execute(Compilation compilation, ImmutableArray<MethodDeclarationSyntax> methods, SourceProductionContext context)
    {
        if (methods.IsDefaultOrEmpty)
        {
            return;
        }
        
        foreach (var filter in methods)
        {
            GenerateFilterFactory(compilation, filter, context);
        }
    }
    
    private static void GenerateFilterFactory(Compilation compilation, MethodDeclarationSyntax filter, SourceProductionContext context)
    {
        var containingNamespace = filter.GetContainingNamespace();
        
        var containingClassSyntax = filter.Parent switch
        {
            ClassDeclarationSyntax cds => cds,
            _ => throw new InvalidOperationException("Filter must be declared in a partial class.")
        };

        if (containingClassSyntax.Modifiers.All(a => !a.IsKind(SyntaxKind.PartialKeyword)))
        {
            return;
        }
        
        var containingClass = containingClassSyntax.Identifier.Text;
        var parameters = filter.ParameterList.Parameters.Select(s => ParameterFactory.GetParameterCodeProvider(s, compilation)).ToArray();
        FilterFactoryBuilder builder = new ();
        
        builder
            .AddFactoryCode(parameters.OfType<IFactoryCodeProvider>().ToImmutableArray())
            .AddFilterConditionCode(parameters.OfType<IFilterConditionProvider>().ToImmutableArray())
            .StartFilterClosure()
            .AddFilterCode(parameters.OfType<IFilterCodeProvider>().ToImmutableArray())
            .AddFilterCall(filter.Identifier.Text, parameters.ToImmutableArray())
            .EndFilterClosure()
            .EndFilterCondition();
        
        var codeText = TypeTemplates.ArgumentativeFilterTemplate(containingNamespace, containingClass, builder.Build());
        context.AddSource($"ArgumentativeFilterFactory.{containingClass}.g.cs", SourceText.From(codeText, Encoding.UTF8));
    }
}