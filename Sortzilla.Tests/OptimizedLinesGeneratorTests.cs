using Sortzilla.Core.Generator;
using Sortzilla.Tests.TestUtils;

namespace Sortzilla.Tests;

internal class OptimizedLinesGeneratorTests
{
    private static readonly TestSequenceSource<int> _digitsSource = new([1, 2, 3]);
    private static readonly TestSequenceSource<string> _wordsSource = new(["one", "two", "three"]);
    private static readonly OptimizedLinesGenerator _generator = new(
        _digitsSource,
        new TestWordsGenerator(_wordsSource));

    [Test]
    [Arguments(-1)]
    [Arguments(0)]
    public async Task GenerateLines_WhenInvalidLength_Throws(long length)
    {
        var action = () => { _generator.GenerateLines(length, _ => { } ); };

        await Assert.That(action).Throws<ArgumentException>();
    }

    [Test]
    public async Task GenerateLines_WhenCalled_GeneratesLines()
    {
        var lines = new List<string>();
        _generator.GenerateLines(18, line => lines.Add(line.ToString()));

        var expectedLines = new[]
        {
            $"1. one{Environment.NewLine}",
            $"2. two{Environment.NewLine}",
            $"3. three{Environment.NewLine}"
        };

        await Assert.That(lines).IsEquivalentTo(expectedLines);
    }
}

