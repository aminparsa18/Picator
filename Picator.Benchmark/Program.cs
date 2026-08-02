using BenchmarkDotNet.Running;

BenchmarkSwitcher.FromAssembly(typeof(EntryPoint).Assembly).Run(args);

internal sealed class EntryPoint;
