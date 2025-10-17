namespace Sortzilla.Core.Generator;

internal class StringPartWriter(ISequenceSource<string> dictionarySource) : IStringPartWriter
{
    private readonly Random _random = new Random();

    public int WriteStringPart(Span<char> buffer)
    {
        int index = 0;
        // randomly select a length between half and full buffer size
        int requiredLength = _random.Next(buffer.Length / 2, buffer.Length);
        while (index < requiredLength)
        {
            // add space between words
            if(index > 0)
                buffer[index++] = ' ';

            var nextWord = dictionarySource.Next();
            
            // if the word doesn't fit, simply break
            if (index + nextWord.Length >= requiredLength)
                break;

            nextWord.CopyTo(buffer[index..]);

            // the first character is always uppercase
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
