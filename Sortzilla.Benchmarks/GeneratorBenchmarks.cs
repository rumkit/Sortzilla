using BenchmarkDotNet.Attributes;
using Sortzilla.Core;
using Sortzilla.Core.Generator;
using System.Security.Cryptography;

namespace Sortzilla.Benchmarks;

[MemoryDiagnoser]
public class GeneratorBenchmarks
{
    private readonly SimpleLinesGenerator _linesGenerator = new SimpleLinesGenerator(new RandomNumberSource(), new RandomStringSource());
    private const string TestFilePath = "testfile.txt";
    private static readonly SimpleLinesGenerator _generator = new(
        new RandomNumberSource(),
        new RandomStringSource()
    );
    private static readonly SpanWriter _spanWriter = new();

    //[Benchmark]
    //public string RandomString() => _linesGenerator.GenerateLines(1).First();


    //[Benchmark]
    //[IterationCount(1)]
    //public int SeveralStrings() {
    //    int count = 0;
    //    foreach (var line in _linesGenerator.GenerateLines(100_000_000))
    //    {
    //        count++;
    //    }

    //    return count;
    //}

    [Benchmark]
    [IterationCount(1)]
    public async Task WriteToFile()
    {
        const long targetSize = 1_00_000_000; // ~1 GB
        using var fStream = File.Create(TestFilePath);
        using var writer = new StreamWriter(fStream, bufferSize: 10_000_000);
        {
            foreach (var line in _generator.GenerateLines(targetSize))
            {
                await writer.WriteAsync(line);
            }

            await writer.FlushAsync();
        }
    }

    [Benchmark]
    [IterationCount(1)]
    public void WriteToFile2()
    {
        const long targetSize = 1_00_000_000; // ~1 GB
        _spanWriter.Write(TestFilePath, targetSize);
    }
}
