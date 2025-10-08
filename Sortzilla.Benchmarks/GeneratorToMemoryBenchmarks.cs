using BenchmarkDotNet.Attributes;
using Sortzilla.Core.Generator;

namespace Sortzilla.Benchmarks;

[MemoryDiagnoser]
public class GeneratorToMemoryBenchmarks
{
    private readonly SimpleLinesGenerator _simpleLinesGenerator = new SimpleLinesGenerator(new RandomNumberSource(), new RandomStringSource());
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
            new RandomWordsGenerator()
,
            new RandomNumberSource());

        _linesGeneratorWithCachedRandomWords = new OptimizedLinesGenerator(
            new DictionaryWordsGenerator()
,
            new RandomNumberSource());

        var dictionary = File.ReadAllLines("english-10k-sorted.txt").ToHashSet();
        _linesGeneratorWithDictionary = new OptimizedLinesGenerator(
            new DictionaryWordsGenerator(dictionary)
,
            new RandomNumberSource());
    }


    [Benchmark(Baseline = true)]
    public int SimpleGenerator()
    {
        int count = 0;
        foreach (var line in _generator.GenerateLines(TargetSize))
        {
            count++;
        }

        return count;
    }

    [Benchmark]
    public int LinesGeneratorRandom()
    {
        int count = 0;
        _linesGeneratorWithRandomWords.GenerateLines(TargetSize, _ => count++);
        return count;
    }

    [Benchmark]
    public int LinesGeneratorCached()
    {
        int count = 0;
        _linesGeneratorWithCachedRandomWords.GenerateLines(TargetSize, _ => count++);
        return count;
    }

    [Benchmark]
    public int LinesGeneratorCachedWithInit()
    {
        var generator = new OptimizedLinesGenerator(
            new DictionaryWordsGenerator()
,
            new RandomNumberSource());
        int count = 0;
        generator.GenerateLines(TargetSize, _ => count++);
        return count;
    }

    [Benchmark]
    public int LinesGeneratorDictionary()
    {
        int count = 0;
        _linesGeneratorWithDictionary.GenerateLines(TargetSize, _ => count++);
        return count;
    }

    [Benchmark]
    public int LinesGeneratorDictionaryWithInit()
    {
        var dictionary = File.ReadAllLines("english-10k-sorted.txt").ToHashSet();
        var generator = new OptimizedLinesGenerator(
            new DictionaryWordsGenerator(dictionary)
,
            new RandomNumberSource());
        int count = 0;
        generator.GenerateLines(TargetSize, _ => count++);
        return count;
    }
}
