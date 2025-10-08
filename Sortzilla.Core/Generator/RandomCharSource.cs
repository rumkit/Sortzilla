namespace Sortzilla.Core.Generator;

public class RandomCharSource : ISequenceSource<char>
{
    private const string Chars = "abcdefghijklmnopqrstuvwxyz";
    private Random _random = new Random();
    public char Next()
    {
        return Chars[_random.Next(Chars.Length)];
    }
}
