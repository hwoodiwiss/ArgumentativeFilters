using System.Collections.Immutable;
using System.Text;

using ArgumentativeFilters.CodeGeneration;
using ArgumentativeFilters.CodeGeneration.Parameters;
using ArgumentativeFilters.CodeGeneration.Parameters.Abstract;

namespace ArgumentativeFilters;

public class FilterFactoryBuilder
{
    private const string FactoryIndentation = "            ";
    private const string ConditionalIndentation = "                ";
    private const string FilterIndentation = "                    ";

    readonly StringBuilder _builder = new();
    
    public FilterFactoryBuilder AddFactoryCode(IImmutableList<IFactoryCodeProvider> factoryCodeProviders)
    {
        foreach (var factoryCodeProvider in factoryCodeProviders)
        {
            _builder.Append(FactoryIndentation);
            _builder.AppendLine(factoryCodeProvider.FactoryCode);
        }

        return this;
    }
    
    public FilterFactoryBuilder AddFilterConditionCode(IImmutableList<IFilterConditionProvider> filterConditionProviders)
    {
        _builder.Append(FactoryIndentation);
        _builder.Append($"if (");
        
        for(var i = 0; i < filterConditionProviders.Count; i++)
        {
            if (i > 0)
            {
                _builder.Append(" && ");
            }
            _builder.Append(filterConditionProviders[i].FilterConditionCode);
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