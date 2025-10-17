using BenchmarkDotNet.Attributes;
using Sortzilla.Core.Generator;

namespace Sortzilla.Benchmarks;

[MemoryDiagnoser]
public class GeneratorToFileBenchmarks
{
    private const string TestFilePath = "testfile.txt";
    const long TargetSize = 100_000_000; // ~100 MB
    private static readonly SimpleLinesGenerator Generator = new(
        new RandomPositiveNumberSource(),
        new RandomStringSource()
    );
    private OptimizedLinesGenerator _linesGeneratorWithRandomWords;
    private OptimizedLinesGenerator _linesGeneratorWithCachedRandomWords;
    private OptimizedLinesGenerator _linesGeneratorWithDictionary;


    [GlobalSetup]
    public void GlobalSetup()
    {
        _linesGeneratorWithRandomWords = new OptimizedLinesGenerator(new RandomStringPartWriter());
        _linesGeneratorWithCachedRandomWords = new OptimizedLinesGenerator(new RandomCachedDictionaryStringSource());

        var dictionary = File.ReadAllLines("english-10k-sorted.txt");
        _linesGeneratorWithDictionary = new OptimizedLinesGenerator(new StaticDictionaryStringSource(dictionary));
    }


    [Benchmark]
    [IterationCount(1)]
    public async Task SimpleGenerator()
    {
        await using var fStream = File.Create(TestFilePath);
        await using var writer = new StreamWriter(fStream, bufferSize: 10_000_000);
        
        foreach (var line in Generator.GenerateLines(TargetSize))
        {
            await writer.WriteAsync(line);
        }

        await writer.FlushAsync();
    }

    [Benchmark]
    [IterationCount(1)]
    public async Task LinesGeneratorRandom()
    {
        await using var fStream = File.Create(TestFilePath);
        await using var writer = new StreamWriter(fStream, bufferSize: 10_000_000);
        
        _linesGeneratorWithRandomWords.GenerateLines(TargetSize, writer.Write);
        await writer.FlushAsync();
    }

    [Benchmark]
    [IterationCount(1)]
    public async Task LinesGeneratorCached()
    {
        await using var fStream = File.Create(TestFilePath);
        await using var writer = new StreamWriter(fStream, bufferSize: 10_000_000);
        
        _linesGeneratorWithCachedRandomWords.GenerateLines(TargetSize, writer.Write);
        await writer.FlushAsync();
    }

    [Benchmark]
    [IterationCount(1)]
    public async Task LinesGeneratorDictionary()
    {
        await using var fStream = File.Create(TestFilePath);
        await using var writer = new StreamWriter(fStream, bufferSize: 10_000_000);
        
        _linesGeneratorWithDictionary.GenerateLines(TargetSize, writer.Write);
        await writer.FlushAsync();
    }
}
