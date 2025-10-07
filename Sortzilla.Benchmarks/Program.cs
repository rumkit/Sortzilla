using BenchmarkDotNet.Running;
using Sortzilla.Benchmarks;

var summary = BenchmarkRunner.Run<GeneratorToMemoryBenchmarks>();
