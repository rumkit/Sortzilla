using Sortzilla.Core.Generator;

namespace Sortzilla.Tests.TestUtils;

internal class TestSequenceSource<T>(T[] elements) : ISequenceSource<T>
{
    private int _currentIndex = -1;
    public T Next()
    {
        _currentIndex++;
        _currentIndex %= elements.Length;
        return elements[_currentIndex];        
    }
}

