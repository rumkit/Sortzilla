using BenchmarkDotNet.Attributes;
using Sortzilla.Core.Generator;

namespace Sortzilla.Benchmarks;

[MemoryDiagnoser]
public class GeneratorToMemoryBenchmarks
{
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


    [Benchmark(Baseline = true)]
    public int SimpleGenerator()
    {
        int count = 0;
        foreach (var _ in Generator.GenerateLines(TargetSize))
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
        var generator = new OptimizedLinesGenerator(new RandomCachedDictionaryStringSource());
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
        var dictionary = File.ReadAllLines("english-10k-sorted.txt");
        var generator = new OptimizedLinesGenerator(new StaticDictionaryStringSource(dictionary));
        int count = 0;
        generator.GenerateLines(TargetSize, _ => count++);
        return count;
    }
}
