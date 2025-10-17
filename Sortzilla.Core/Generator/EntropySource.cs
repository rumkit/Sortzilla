namespace Sortzilla.Core.Generator;

public class EntropySource(int everyNth = 1000) : ISequenceSource<bool>
{
    private readonly Random _random = new ();
    public bool Next() => _random.Next() % everyNth == 0;
}
