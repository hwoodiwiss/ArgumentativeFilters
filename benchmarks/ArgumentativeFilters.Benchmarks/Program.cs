using BenchmarkDotNet.Running;

BenchmarkSwitcher.FromAssembly(typeof(ArgumentativeFilters.Benchmarks.FilterAndMiddlewareBenchmarks).Assembly).Run(args);
