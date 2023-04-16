using System.Collections.Immutable;
using System.Text;

using ArgumentativeFilters.CodeGeneration.Parameters;
using ArgumentativeFilters.CodeGeneration.Parameters.Abstract;

namespace ArgumentativeFilters.CodeGeneration;

public class FilterFactoryBuilder
{
    const string EndpointFilterFactoryContextType = "global::Microsoft.AspNetCore.Http.EndpointFilterFactoryContext";

    
    readonly StringBuilder _builder;
    private readonly string _startingIndentation;
    private readonly string _factoryIndentation;
    private string _filterIndentation;
    private string _conditionalIndentation;

    
    private bool _hasCondition = true;
    
    public FilterFactoryBuilder(StringBuilder builder, int startingIndentation)
    {
        _builder = builder;

        _startingIndentation = new string(' ', startingIndentation);
        _factoryIndentation = new string(' ', startingIndentation + Constants.IndentationPerLevel);
        _conditionalIndentation = new string(' ', startingIndentation + Constants.IndentationPerLevel + Constants.IndentationPerLevel);
        _filterIndentation = new string(' ', startingIndentation + Constants.IndentationPerLevel + Constants.IndentationPerLevel + Constants.IndentationPerLevel);
    }

    public FilterFactoryBuilder AddFilterFactorySignature(string filterClassAccessibility)
    {
        string endpointFilterDelegateType = "global::Microsoft.AspNetCore.Http.EndpointFilterDelegate";
        _builder.AppendLine($"{_startingIndentation}{filterClassAccessibility} static {endpointFilterDelegateType} Factory({EndpointFilterFactoryContextType} {VariableNames.FactoryFilterContext}," +
                            $" {endpointFilterDelegateType} {VariableNames.EndpointFilterDelegate})");
        _builder.AppendLine($"{_startingIndentation}{{");

        return this;
    }
    
    public FilterFactoryBuilder AddFactoryCode(IImmutableList<IFactoryCodeProvider> factoryCodeProviders)
    {
        foreach (var factoryCode in factoryCodeProviders.Select(x => x.FactoryCode).Distinct())
        {
            _builder.Append(_factoryIndentation);
            _builder.AppendLine(factoryCode);
        }

        return this;
    }
    
    public FilterFactoryBuilder AddFilterConditionCode(IImmutableList<IFilterConditionProvider> filterConditionProviders)
    {
        var distinctConditions = filterConditionProviders.Select(s => s.FilterConditionCode).Where(w => w != string.Empty).Distinct().ToArray();

        if (distinctConditions.Length == 0)
        {
            // Reduce indentation by one level for everything under the non-existent condition
            _filterIndentation = _conditionalIndentation;
            _conditionalIndentation = _factoryIndentation;
            _hasCondition = false;
            return this;
        }
        
        _builder.Append(_factoryIndentation);
        _builder.Append($"if (");
        
        for(var i = 0; i < distinctConditions.Length; i++)
        {
            if (i > 0)
            {
                _builder.Append(" && ");
            }
            _builder.Append(distinctConditions[i]);
        }
        
        _builder.AppendLine($")");
        _builder.Append(_factoryIndentation);
        _builder.AppendLine("{");

        return this;
    }
    
    public FilterFactoryBuilder StartFilterClosure()
    {
        _builder.Append(_conditionalIndentation);
        _builder.AppendLine($"return {VariableNames.InvocationFilterContext} => {{");

        return this;
    }
    
    public FilterFactoryBuilder AddFilterCode(IImmutableList<IFilterCodeProvider> filterCodeProviders)
    {
        foreach (var filterCodeProvider in filterCodeProviders)
        {
            _builder.Append(_filterIndentation);
            _builder.AppendLine(filterCodeProvider.FilterCode);
        }

        return this;
    }
    
    public FilterFactoryBuilder AddFilterCall(string filterName, IImmutableList<ArgumentativeFilterParameterProvider> filterParameterProviders)
    {
        _builder.Append(_filterIndentation);
        _builder.Append($"return {filterName}(");
        
        for(var i = 0; i < filterParameterProviders.Count; i++)
        {
            if (i > 0)
            {
                _builder.Append(", ");
            }
            _builder.Append(filterParameterProviders[i].ParameterCode);
        }
        
        _builder.AppendLine(");");

        return this;
    }
    
    public FilterFactoryBuilder EndFilterClosure()
    {
        _builder.Append(_conditionalIndentation);
        _builder.AppendLine("};");

        return this;
    }

    public FilterFactoryBuilder EndFilterCondition()
    {
        if (!_hasCondition)
        {
            return this;
        }
        
        _builder.Append(_factoryIndentation);
        _builder.AppendLine("}");

        return this;
    }

    public FilterFactoryBuilder EndFilterFactory()
    {
        if (_hasCondition)
        {
            _builder.AppendLine($"{_factoryIndentation}return {VariableNames.InvocationFilterContext} => {VariableNames.EndpointFilterDelegate}({VariableNames.InvocationFilterContext});");
        }
        _builder.Append(_startingIndentation);
        _builder.AppendLine("}");
        _builder.AppendLine();

        return this;
    }
    
    public FilterFactoryBuilder AddGetArgumentIndexMethod()
    {
        _builder.Append(_startingIndentation);
        _builder.AppendLine($"private static int? GetArgumentIndex({EndpointFilterFactoryContextType} context, string argumentName)");
        _builder.Append(_factoryIndentation);
        _builder.AppendLine("=> context.MethodInfo.GetParameters().FirstOrDefault(p => string.Equals(p.Name, argumentName, StringComparison.Ordinal))?.Position;");
        
        return this;
    }
}