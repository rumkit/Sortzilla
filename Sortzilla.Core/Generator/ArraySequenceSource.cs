namespace Sortzilla.Core.Generator;

public abstract class ArraySequenceSource<T>(T[] source) : ISequenceSource<T>
{
    private readonly Random _random = new ();
    private readonly int _length = source.Length;

    public T Next()
    {
        return source[_random.Next(_length)];
    }
}