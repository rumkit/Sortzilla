using Sortzilla.Core.Generator;
using Sortzilla.Tests.TestUtils;

namespace Sortzilla.Tests;

internal class StringPartWriterTests
{
    [Test]
    public async Task WriteStringPart_WhenCalled_ShouldUseCorrectFormat()
    {
        var writer = new StringPartWriter(new TestSequenceSource<string>(["one", "two", "three"]));
        Span<char> buffer = stackalloc char[100];

        int written = writer.WriteStringPart(buffer);
        var result = new string(buffer[..written]);

        await Assert.That(written).IsBetween(46, 100);
        await Assert.That(result[0]).IsUpper();
        await Assert.That(result).DoesNotEndWith(' ');
    }
}

