using Sortzilla.Core.Generator;

namespace Sortzilla.Tests.TestUtils;

internal class TestWordsGenerator(ISequenceSource<string> source) : IWordsGenerator
{
    public int WriteWordsToBuffer(Span<char> buffer)
    {
        var word = source.Next();
        word.CopyTo(buffer);

        return word.Length;
    }
}
