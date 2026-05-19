using System.Collections.Immutable;
using System.Text;

namespace ArgumentativeFilters.Generator.CodeGeneration;

public class MiddlewareBuilder
{
    const string RequestDelegateType = "global::Microsoft.AspNetCore.Http.RequestDelegate";

    readonly StringBuilder _builder;
    private readonly string _startingIndentation;
    private readonly string _middlewareIndentation;
    private readonly string _invocationIndentation;

    public MiddlewareBuilder(StringBuilder builder, int startingIndentation)
    {
        _builder = builder;

        _startingIndentation = new string(' ', startingIndentation);
        _middlewareIndentation = new string(' ', startingIndentation + Constants.IndentationPerLevel);
        _invocationIndentation = new string(' ', startingIndentation + Constants.IndentationPerLevel + Constants.IndentationPerLevel);
    }

    public MiddlewareBuilder AddGeneratedCodeAttribute()
    {
        _builder.AppendLine($"{_startingIndentation}[global::System.CodeDom.Compiler.GeneratedCodeAttribute(\"{AssemblyMetadata.Name}\", \"{AssemblyMetadata.Version}\")]");

        return this;
    }

    public MiddlewareBuilder AddMiddlewareSignature(string middlewareClassAccessibility, string? middlewareNamePrefix)
    {
        _builder.AppendLine($"{_startingIndentation}{middlewareClassAccessibility} static {RequestDelegateType} {middlewareNamePrefix}Middleware({RequestDelegateType} {VariableNames.RequestDelegate})");
        _builder.AppendLine($"{_startingIndentation}{{");

        return this;
    }

    public MiddlewareBuilder StartMiddlewareClosure()
    {
        _builder.Append(_middlewareIndentation);
        _builder.AppendLine($"return {VariableNames.InvocationHttpContext} =>");
        _builder.Append(_middlewareIndentation);
        _builder.AppendLine("{");

        return this;
    }

    public MiddlewareBuilder AddMiddlewareCode(string middlewareCode)
    {
        _builder.Append(_invocationIndentation);
        _builder.AppendLine(middlewareCode);

        return this;
    }

    public MiddlewareBuilder AddMiddlewareCall(string middlewareName, IImmutableList<string> middlewareParameterProviders, bool returnsValueTask)
    {
        _builder.Append(_invocationIndentation);
        _builder.Append($"return {middlewareName}(");

        for (var i = 0; i < middlewareParameterProviders.Count; i++)
        {
            if (i > 0)
            {
                _builder.Append(", ");
            }
            _builder.Append(middlewareParameterProviders[i]);
        }

        _builder.AppendLine(returnsValueTask ? ").AsTask();" : ");");

        return this;
    }

    public MiddlewareBuilder EndMiddlewareClosure()
    {
        _builder.Append(_middlewareIndentation);
        _builder.AppendLine("};");

        return this;
    }

    public MiddlewareBuilder EndMiddleware()
    {
        _builder.Append(_startingIndentation);
        _builder.AppendLine("}");
        _builder.AppendLine();

        return this;
    }
}
