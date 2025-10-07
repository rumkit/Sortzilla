namespace Sortzilla.Core.Generator;

public class OptimizedLinesGenerator(ISequenceSource<int> numbersSource, IWordsGenerator wordsGenerator)
{
    private const int MaxLineLength = 100;

    public void GenerateLines(long requiredTotalLength, LinesGeneratorHandler lineHandler)
    {
        if (requiredTotalLength < 1)
            throw new ArgumentOutOfRangeException(nameof(requiredTotalLength), "Must be positive");

        long currentLength = 0;

        ReadOnlySpan<char> newlineSpan = Environment.NewLine.AsSpan();
        Span<char> lineBuffer = stackalloc char[MaxLineLength];
        

        while (currentLength < requiredTotalLength)
        {
            var nextNumber = numbersSource.Next();
            if (!nextNumber.TryFormat(lineBuffer, out int charsWritten))            
                throw new InvalidOperationException($"Can't properly format {nextNumber}");
            
            charsWritten += wordsGenerator.WriteWordsToBuffer(lineBuffer[charsWritten .. ^newlineSpan.Length]);
            newlineSpan.CopyTo(lineBuffer[charsWritten ..]);
            charsWritten += newlineSpan.Length;

            lineHandler(lineBuffer[..charsWritten]);
            currentLength += charsWritten;
        }
    }
}

public delegate void LinesGeneratorHandler(ReadOnlySpan<char> line);
