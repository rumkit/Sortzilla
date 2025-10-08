namespace Sortzilla.Core.Generator;

public interface IStringPartWriter
{
    int WriteStringPart(Span<char> buffer);
}
