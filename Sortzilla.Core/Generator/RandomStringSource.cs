using System.Text;

namespace Sortzilla.Core.Generator;

public class RandomStringSource : ISequenceSource<string>
{
    private Random _random = new Random();
    private const string _chars = "abcdefghijklmnopqrstuvwxyz";
    private readonly int _minLength;
    private readonly int _maxLength;

    public RandomStringSource(int minLength = 15, int maxLength = 100)
    {
        if(minLength < 0)
            throw new ArgumentOutOfRangeException(nameof(minLength), "Must be non-negative");
        if(maxLength < minLength)
            throw new ArgumentOutOfRangeException(nameof(maxLength), $"Must be greater than {nameof(minLength)}");

        _minLength = minLength;
        _maxLength = maxLength;
    }

    private char GetNextChar() => _chars[_random.Next(_chars.Length)];
    public string Next()
    {
        int requiredLength = _random.Next(_minLength, _maxLength + 1);
        var sBuilder = new StringBuilder(requiredLength);

        while(sBuilder.Length < requiredLength)
        {
            // first character is always uppercase
            if (sBuilder.Length == 0)
            {
                sBuilder.Append(char.ToUpper(_chars[_random.Next(_chars.Length)]));
                continue;
            }

            // every ~5th character is a space
            if (_random.Next() % 5 == 0 && sBuilder[sBuilder.Length - 1] != ' ') 
            {
                sBuilder.Append(' ');
            }
            else
            {
                sBuilder.Append(GetNextChar());
            }
        }
        
        return sBuilder.ToString();
    }
}