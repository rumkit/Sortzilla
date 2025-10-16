namespace Sortzilla.Core.Generator;

internal interface IStringPartWriter
{
    int WriteStringPart(Span<char> buffer);
}
