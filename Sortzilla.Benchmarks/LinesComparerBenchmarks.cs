using BenchmarkDotNet.Attributes;
using Sortzilla.Core;
using Sortzilla.Core.Validator;

namespace Sortzilla.Benchmarks;

[MemoryDiagnoser]
public class LinesComparerBenchmarks
{
    private const string TestSample = """
        1119782830. ARoger mostly searches cheap reveals kernel
        772399000. BFreeware estonia journal platinum parliamentary promised quest renewal this
        1720963445. CGuitar crucial exchange destruction amount annie collectors disciplines
        2125588986. DEconomic highlighted ladder albert
        2109749920. EHeadline vic column ut antigua warren pricing bin advert durham older sanyo
        185719021. FLocations marcus retain cir physiology struct camps gilbert
        906576720. GReflection smithsonian maintain ph dicke joint
        1840748938. GReflection smithsonian maintain ph dicke joint
        1967042264. IMontreal activity automobiles peace gbp triangle randy toronto boats bulletin
        1002880194. JLiterally von officer raises learned via instrumentation transcripts
        1930581224. KArgument fatal records interval literally
        1719978313. LDimensions attach prediction factor brake selective ref info buyer feb spiritual
        1508421561. Mambda exotic sharp asia lcd consent beneath athens
        1032554530. NEnables furniture single bloomberg involved qty limit get column cowboy moment        
        """;

    private static readonly string[] input = TestSample.Split(Environment.NewLine);
    private readonly LinesComparer _linesComparer = new ();
    private readonly OptimizedLinesComparer _optimizedLinesComparer = new();

    [Benchmark(Baseline = true)]
    public int SimpleLinesComparer()
    {
        var sum = 0;
        for (int i = 0; i < input.Length - 1; i++)
        {
            sum += _linesComparer.Compare(input[i], input[i + 1]);
        }

        return sum;
    }

    [Benchmark]
    public int OptimizedLinesComparer()
    {
        var sum = 0;
        for (int i = 0; i < input.Length - 1; i++)
        {
            sum += _optimizedLinesComparer.Compare(input[i], input[i + 1]);
        }

        return sum;
    }
}
