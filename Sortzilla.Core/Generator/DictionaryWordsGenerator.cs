using System.Collections.ObjectModel;

namespace Sortzilla.Core.Generator;

public class DictionaryWordsGenerator : IWordsGenerator
{
    private const string Chars = "abcdefghijklmnopqrstuvwxyz";
    private const int MaxWordLength = 10;
    private const int MinWordLength = 3;

    private readonly Random _random = new Random();
    private readonly int _dictionarySize;
    private ReadOnlyCollection<string> _dictionary = Array.Empty<string>().AsReadOnly();
    private bool _isDictionarySeeded = false;

    public DictionaryWordsGenerator(int dictionarySize = 100_000)
    {
        _dictionarySize = dictionarySize;
    }

    public DictionaryWordsGenerator(HashSet<string> dictionary) : this(-1) 
    {
        if (dictionary == null || dictionary.Count == 0)
            throw new ArgumentException("Dictionary cannot be null or empty", nameof(dictionary));

        _dictionary = dictionary.ToArray().AsReadOnly();
        _isDictionarySeeded = true;
    }


    // todo: can be replaced with ISequenceSource<string> for both cases
    private char GetNextChar() => Chars[_random.Next(Chars.Length)];
    private string GetNextWord() => _dictionary[_random.Next(_dictionary.Count)];

    private void SeedDictionary()
    {
        Span<char> buffer = stackalloc char[MaxWordLength];
        var hashSet = new HashSet<string>();

        while (hashSet.Count < _dictionarySize)
        {
            int wordLength = _random.Next(MinWordLength, MaxWordLength + 1);
            for (int i = 0; i < wordLength; i++)
            {
                buffer[i] = GetNextChar();
            }
            hashSet.Add(new string(buffer[..wordLength]));
        }

        _dictionary = hashSet.ToArray().AsReadOnly();

        _isDictionarySeeded = true;
    }

    public int WriteWordsToBuffer(Span<char> buffer)
    {
        if (!_isDictionarySeeded)
            SeedDictionary();

        int index = 0;
        // randomly select length between half and full buffer size
        int requiredLength = _random.Next(buffer.Length / 2, buffer.Length);
        while (index < requiredLength)
        {
            // add space between words
            if(index > 0)
                buffer[index++] = ' ';

            var nextWord = GetNextWord();
            
            // if the word doesn't fit, simply break
            if (index + nextWord.Length >= requiredLength)
                break;

            nextWord.CopyTo(buffer[index..]);

            // first character is always uppercase
            if (index == 0)
                buffer[index] = char.ToUpper(buffer[index]);

            index += nextWord.Length;
        }

        return index;
    }
}
