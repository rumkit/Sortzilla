namespace Sortzilla.Core.Generator;

public class RandomCharSource : ISequenceSource<char>
{
    private const string Chars = "abcdefghijklmnopqrstuvwxyz";
    private readonly Random _random = new ();
    public char Next()
    {
        return Chars[_random.Next(Chars.Length)];
    }
}
