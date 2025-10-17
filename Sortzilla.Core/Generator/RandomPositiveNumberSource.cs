namespace Sortzilla.Core.Generator;

public class RandomPositiveNumberSource : ISequenceSource<int>
{
    private readonly Random _random = new ();

    public int Next()
    {
        return _random.Next(1, int.MaxValue);
    }
}
