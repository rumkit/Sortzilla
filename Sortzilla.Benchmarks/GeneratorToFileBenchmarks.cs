using BenchmarkDotNet.Attributes;
using Sortzilla.Core.Generator;

namespace Sortzilla.Benchmarks;

[MemoryDiagnoser]
public class GeneratorToFileBenchmarks
{
    private readonly SimpleLinesGenerator _simpleLinesGenerator = new SimpleLinesGenerator(new RandomNumberSource(), new RandomStringSource());
    private const string TestFilePath = "testfile.txt";
    const long TargetSize = 100_000_000; // ~100 MB
    private static readonly SimpleLinesGenerator _generator = new(
        new RandomNumberSource(),
        new RandomStringSource()
    );
    private OptimizedLinesGenerator _linesGeneratorWithRandomWords;
    private OptimizedLinesGenerator _linesGeneratorWithCachedRandomWords;
    private OptimizedLinesGenerator _linesGeneratorWithDictionary;


    [GlobalSetup]
    public void GlobalSetup()
    {
        _linesGeneratorWithRandomWords = new OptimizedLinesGenerator(
            new RandomNumberSource(),
            new RandomWordsGenerator()
        );

        _linesGeneratorWithCachedRandomWords = new OptimizedLinesGenerator(
            new RandomNumberSource(),
            new DictionaryWordsGenerator()
        );

        var dictionary = File.ReadAllLines("english-10k-sorted.txt").ToHashSet();
        _linesGeneratorWithDictionary = new OptimizedLinesGenerator(
            new RandomNumberSource(),
            new DictionaryWordsGenerator(dictionary)
        );
    }


    [Benchmark]
    [IterationCount(1)]
    public async Task SimpleGenerator()
    {
        
        using var fStream = File.Create(TestFilePath);
        using var writer = new StreamWriter(fStream, bufferSize: 10_000_000);
        {
            foreach (var line in _generator.GenerateLines(TargetSize))
            {
                await writer.WriteAsync(line);
            }

            await writer.FlushAsync();
        }
    }

    [Benchmark]
    [IterationCount(1)]
    public async Task LinesGeneratorRandom()
    {
        using var fStream = File.Create(TestFilePath);
        using var writer = new StreamWriter(fStream, bufferSize: 10_000_000);
        {
            _linesGeneratorWithRandomWords.GenerateLines(TargetSize, writer.Write);

            await writer.FlushAsync();
        }
    }

    [Benchmark]
    [IterationCount(1)]
    public async Task LinesGeneratorCached()
    {
        using var fStream = File.Create(TestFilePath);
        using var writer = new StreamWriter(fStream, bufferSize: 10_000_000);
        {
            _linesGeneratorWithCachedRandomWords.GenerateLines(TargetSize, writer.Write);

            await writer.FlushAsync();
        }
    }

    [Benchmark]
    [IterationCount(1)]
    public async Task LinesGeneratorDictionary()
    {
        using var fStream = File.Create(TestFilePath);
        using var writer = new StreamWriter(fStream, bufferSize: 10_000_000);
        {
            _linesGeneratorWithDictionary.GenerateLines(TargetSize, writer.Write);

            await writer.FlushAsync();
        }
    }
}
