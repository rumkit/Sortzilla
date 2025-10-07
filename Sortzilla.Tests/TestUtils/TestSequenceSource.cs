using Sortzilla.Core.Generator;

namespace Sortzilla.Tests.TestUtils;

internal class TestSequenceSource<T>(T[] elements) : ISequenceSource<T>
{
    int currentIndex = -1;
    public T Next()
    {
        currentIndex++;
        currentIndex %= elements.Length;
        return elements[currentIndex];        
    }
}

