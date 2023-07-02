using System.Text;
using ArgumentativeFilters.CodeGeneration.Extensions;

namespace ArgumentativeFilters.CodeGeneration;

public class FilterFactoryContainingHierarchyBuilder
{
    private int _currentIndentationLevel;
    private int _hierarchyLevelCount = 0;
    private readonly Dictionary<int, string> _indentationCache = new();
    private readonly StringBuilder _builder;
    
    public int CurrentIndentationLevel => _currentIndentationLevel;
    
    public FilterFactoryContainingHierarchyBuilder(StringBuilder builder)
    {
        _builder = builder;
        _currentIndentationLevel = 0;
    }

    public FilterFactoryContainingHierarchyBuilder AddContainingHierarchy(IEnumerable<INamedTypeSymbol> containingClassSymbols, string containingNamespace)
    {
        AddContainingNamespace(containingNamespace);
        
        foreach(var containingClassSymbol in containingClassSymbols)
        {
            AddContainingClass(containingClassSymbol);
        }
        
        return this;
    }
    
    private FilterFactoryContainingHierarchyBuilder AddContainingNamespace(string containingNamespace)
    {
        _builder.AppendLine($"namespace {containingNamespace}");
        _builder.AppendLine("{");
        _currentIndentationLevel += Constants.IndentationPerLevel;
        return this;
    }

    private FilterFactoryContainingHierarchyBuilder AddContainingClass(INamedTypeSymbol containingClassSymbol)
    {
        _hierarchyLevelCount++;
        
        var indentation = GetOrCreateIndentationLevel(_currentIndentationLevel);
        var containingClassAccessibility = containingClassSymbol.GetAccessibilityString();
        
        var containingTypeStatic = containingClassSymbol.IsStatic ? "static " : string.Empty;
        
        var containingTypeKind = containingClassSymbol.TypeKind switch
        {
            TypeKind.Class => "class",
            TypeKind.Struct => "struct",
            _ => throw new InvalidOperationException("Filter must be declared in a class or struct.")
        };
        
        var containingClassName = containingClassSymbol.Name;
        
        _builder.AppendLine($"{indentation}{containingClassAccessibility} {containingTypeStatic}partial {containingTypeKind} {containingClassName}");
        _builder.AppendLine($"{indentation}{{");
        
        _currentIndentationLevel += Constants.IndentationPerLevel;
        return this;
    }
    
    public FilterFactoryContainingHierarchyBuilder CloseContainingHierarchy()
    {
        for(var i = 1; i < _hierarchyLevelCount + 1; i++)
        {
            var indentation = GetOrCreateIndentationLevel(_currentIndentationLevel - (i * Constants.IndentationPerLevel));
            _builder.AppendLine($"{indentation}}}");
        }
        
        // Close containing namespace
        _builder.AppendLine("}");

        return this;
    }

    private string GetOrCreateIndentationLevel(int indentationLevel)
    {
        if (_indentationCache.TryGetValue(indentationLevel, out var indentation))
        {
            return indentation;
        }

        var indentationString = new string(' ', indentationLevel);
        _indentationCache.Add(indentationLevel, indentationString);
        return indentationString;
    }
    
    public string Build()
    {
        return _builder.ToString();
    }
}