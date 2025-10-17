using Sortzilla.Core.Generator;
using Sortzilla.Tests.TestUtils;

namespace Sortzilla.Tests;

internal class SimpleLinesGeneratorTests
{
    private static readonly TestSequenceSource<int> DigitsSource = new([1, 2, 3]);
    private static readonly TestSequenceSource<string> WordsSource = new(["one", "two", "three"]);
    private static readonly SimpleLinesGenerator Generator = new SimpleLinesGenerator(DigitsSource, WordsSource);

    [Test]
    [Arguments(-1)]
    [Arguments(0)]
    public async Task GenerateLines_WhenInvalidLength_Throws(long length)
    {
        var action = () => { Generator.GenerateLines(length).First(); };

        await Assert.That(action).Throws<ArgumentException>();
    }

    [Test]
    public async Task GenerateLines_WhenCalled_GeneratesLines()
    {
        var lines = Generator.GenerateLines(18).ToArray();
        var expectedLines = new[]
        {
            $"1. one{Environment.NewLine}",
            $"2. two{Environment.NewLine}",
            $"3. three{Environment.NewLine}"
        };

        await Assert.That(lines).IsEquivalentTo(expectedLines);
    }
}

