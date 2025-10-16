using Sortzilla.Core;
using Sortzilla.Core.Generator;
using Sortzilla.Tests.TestUtils;

namespace Sortzilla.Tests;

internal class LinesComparerTests
{
    private readonly LinesComparer _comparer = new LinesComparer();

    [Test]
    [Arguments("123. Aa", "123. Ab")]
    [Arguments("122. Aa", "123. Aa")]
    [Arguments("122. Aa", "00123. Aa")]
    public async Task Compare_WhenLeftIsLess_ShouldReturnNegative(string left, string right)
    {
        var result = _comparer.Compare(left, right);
        await Assert.That(result).IsLessThan(0);
    }

    [Test]
    [Arguments("123. Aa", "123. Ab")]
    [Arguments("122. Aa", "123. Aa")]
    [Arguments("122. Aa", "00123. Aa")]
    public async Task Compare_WhenLeftIsGreater_ShouldReturnPositive(string left, string right)
    {
        // simply reverse right and left
        var result = _comparer.Compare(right, left);
        await Assert.That(result).IsGreaterThan(0);
    }

    [Test]
    public async Task Compare_WhenLeftAndRightAreEqual_ShouldReturnZero()
    {
        var result = _comparer.Compare("123. Aa", "123. Aa");
        await Assert.That(result).IsEqualTo(0);
    }
}

