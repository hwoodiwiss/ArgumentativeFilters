using System.Collections.Immutable;
using System.Text;

using ArgumentativeFilters.CodeGeneration.Parameters;
using ArgumentativeFilters.CodeGeneration.Parameters.Abstract;

namespace ArgumentativeFilters.CodeGeneration;

public class FilterFactoryBuilder
{
    private const string FactoryIndentation = "            ";
    private const string ConditionalIndentation = "                ";
    private const string FilterIndentation = "                    ";

    readonly StringBuilder _builder = new();
    
    public FilterFactoryBuilder AddFactoryCode(IImmutableList<IFactoryCodeProvider> factoryCodeProviders)
    {
        foreach (var factoryCode in factoryCodeProviders.Select(x => x.FactoryCode).Distinct())
        {
            _builder.Append(FactoryIndentation);
            _builder.AppendLine(factoryCode);
        }

        return this;
    }
    
    public FilterFactoryBuilder AddFilterConditionCode(IImmutableList<IFilterConditionProvider> filterConditionProviders)
    {
        _builder.Append(FactoryIndentation);
        _builder.Append($"if (");
        
        var distinctConditions = filterConditionProviders.Select(s => s.FilterConditionCode).Distinct().ToArray();
        
        for(var i = 0; i < distinctConditions.Length; i++)
        {
            if (i > 0)
            {
                _builder.Append(" && ");
            }
            _builder.Append(distinctConditions[i]);
        }
        
        _builder.AppendLine($")");
        _builder.Append(FactoryIndentation);
        _builder.AppendLine("{");

        return this;
    }
    
    public FilterFactoryBuilder StartFilterClosure()
    {
        _builder.Append(ConditionalIndentation);
        _builder.AppendLine($"return {VariableNames.InvocationFilterContext} => {{");

        return this;
    }
    
    public FilterFactoryBuilder AddFilterCode(IImmutableList<IFilterCodeProvider> filterCodeProviders)
    {
        foreach (var filterCodeProvider in filterCodeProviders)
        {
            _builder.Append(FilterIndentation);
            _builder.AppendLine(filterCodeProvider.FilterCode);
        }

        return this;
    }
    
    public FilterFactoryBuilder AddFilterCall(string filterName, IImmutableList<ArgumentativeFilterParameterProvider> filterParameterProviders)
    {
        _builder.Append(FilterIndentation);
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
        _builder.Append(ConditionalIndentation);
        _builder.AppendLine("};");

        return this;
    }

    public FilterFactoryBuilder EndFilterCondition()
    {
        _builder.Append(FactoryIndentation);
        _builder.AppendLine("}");

        return this;
    }

    public string Build() 
        => _builder.ToString();
}