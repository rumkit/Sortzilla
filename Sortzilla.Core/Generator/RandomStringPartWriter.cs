namespace Sortzilla.Core.Generator;

public class RandomStringPartWriter : IStringPartWriter
{
    private const string Chars = "abcdefghijklmnopqrstuvwxyz";
    private readonly Random _random = new ();
    
    private char GetNextChar() => Chars[_random.Next(Chars.Length)];

    public int WriteStringPart(Span<char> buffer)
    {
        int index = 0;
        // randomly select a length between half and full buffer size
        int requiredLength = _random.Next(buffer.Length / 2, buffer.Length);
        while (index < requiredLength)
        {
            // the first character is always uppercase
            if (index == 0)
            {
                buffer[index++] = char.ToUpper(Chars[_random.Next(Chars.Length)]);
                continue;
            }
            // every ~5th character is a space
            if (_random.Next() % 5 == 0 && buffer[index - 1] != ' ' && index < buffer.Length - 1)
            {
                buffer[index++] = ' ';
            }
            else
            {
                buffer[index++] = GetNextChar();
            }
        }

        return index;
    }    
}
