namespace Sortzilla.Core.Generator;

public interface IWordsGenerator
{
    int WriteWordsToBuffer(Span<char> buffer);
}
