using System.Collections.ObjectModel;

namespace Sortzilla.Core.Generator;

public class RandomCachedDictionaryStringSource(int dictionarySize = 100_000) : ISequenceSource<string>
{
    private const string Chars = "abcdefghijklmnopqrstuvwxyz";
    private const int MaxWordLength = 10;
    private const int MinWordLength = 3;

    private readonly Random _random = new Random();
    private bool _isDictionarySeeded = false;
    private ReadOnlyCollection<string> _dictionary = Array.Empty<string>().AsReadOnly();
    
    public string Next()
    {
        if (!_isDictionarySeeded)
            SeedDictionary();

        return _dictionary[_random.Next(dictionarySize)];
    }

    private char GetNextChar() => Chars[_random.Next(Chars.Length)];

    private void SeedDictionary()
    {
        Span<char> buffer = stackalloc char[MaxWordLength];
        var hashSet = new HashSet<string>();

        while (hashSet.Count < dictionarySize)
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
}
