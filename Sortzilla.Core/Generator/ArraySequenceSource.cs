namespace Sortzilla.Core.Generator;

public abstract class ArraySequenceSource<T>(T[] source) : ISequenceSource<T>
{
    private readonly Random _random = new ();

    public T Next()
    {
        return source[_random.Next(source.Length)];
    }
}