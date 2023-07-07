﻿namespace ArgumentativeFilters.Generator.CodeGeneration;

public static class TypeTemplates
{
    public static string ArgumentativeFilterConstantHeader => $@"// <auto-generated/>
using global::System;
using global::System.Linq;
using global::Microsoft.Extensions.DependencyInjection;

#nullable enable

";
}