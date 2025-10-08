namespace Sortzilla.Core.Generator;

public class RandomPositiveNumberSource : ISequenceSource<int>
{
    private Random _random = new Random();

    public int Next()
    {
        return _random.Next(1, int.MaxValue);
    }
}
