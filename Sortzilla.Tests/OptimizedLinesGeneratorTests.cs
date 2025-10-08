using Sortzilla.Core.Generator;
using Sortzilla.Tests.TestUtils;

namespace Sortzilla.Tests;

internal class OptimizedLinesGeneratorTests
{
    private static readonly TestSequenceSource<int> _digitsSource = new([1, 2, 3]);
    private static readonly TestSequenceSource<bool> entropyFalseSource = new([false]);
    private static readonly TestSequenceSource<bool> entropyTrueSource = new([true]);
    private static readonly TestSequenceSource<string> _wordsSource = new(["one", "two", "three"]);
    private static readonly OptimizedLinesGenerator _generator = new(
        new TestWordsGenerator(_wordsSource),
        _digitsSource,
        entropyFalseSource);

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

    [Test]
    public async Task GenerateLines_WhenPossible_ShouldDuplicateStringPart()
    {
        var generator = new OptimizedLinesGenerator(
            new TestWordsGenerator(_wordsSource),
            _digitsSource,
            entropyTrueSource);
        var lines = new List<string>();
        generator.GenerateLines(18, line => lines.Add(line.ToString()));

        var expectedLines = new[]
        {
            $"1. one{Environment.NewLine}",
            $"2. one{Environment.NewLine}",
            $"3. one{Environment.NewLine}"
        };

        await Assert.That(lines).IsEquivalentTo(expectedLines);
    }
}

