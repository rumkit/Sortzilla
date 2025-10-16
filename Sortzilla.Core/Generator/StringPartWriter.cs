namespace Sortzilla.Core.Generator;

internal class StringPartWriter : IStringPartWriter
{
    private readonly ISequenceSource<string> _dictionarySource;
    private readonly Random _random = new Random();

    public StringPartWriter(ISequenceSource<string> dictionarySource)
    {
        _dictionarySource = dictionarySource;
    }

    public int WriteStringPart(Span<char> buffer)
    {
        int index = 0;
        // randomly select length between half and full buffer size
        int requiredLength = _random.Next(buffer.Length / 2, buffer.Length);
        while (index < requiredLength)
        {
            // add space between words
            if(index > 0)
                buffer[index++] = ' ';

            var nextWord = _dictionarySource.Next();
            
            // if the word doesn't fit, simply break
            if (index + nextWord.Length >= requiredLength)
                break;

            nextWord.CopyTo(buffer[index..]);

            // first character is always uppercase
            if (index == 0)
                buffer[index] = char.ToUpper(buffer[index]);

            index += nextWord.Length;
        }

        // remove trailing space
        if (buffer[index - 1] == ' ')
            index--;

        return index;
    }
}
