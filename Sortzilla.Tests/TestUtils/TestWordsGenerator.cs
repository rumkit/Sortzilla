using Sortzilla.Core.Generator;

namespace Sortzilla.Tests.TestUtils;

internal class TestWordsGenerator(ISequenceSource<string> source) : IStringPartWriter
{
    public int WriteStringPart(Span<char> buffer)
    {
        var word = source.Next();
        word.CopyTo(buffer);

        return word.Length;
    }
}
