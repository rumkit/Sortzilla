using BenchmarkDotNet.Running;
using Sortzilla.Benchmarks;
using Sortzilla.Core;

var summary = BenchmarkRunner.Run<LinesComparerBenchmarks>();
