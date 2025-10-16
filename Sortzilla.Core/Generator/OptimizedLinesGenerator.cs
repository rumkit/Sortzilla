namespace Sortzilla.Core.Generator;

public class OptimizedLinesGenerator
{
    private const int MaxLineLength = Sorter.SortSettingsInternal.MaxLineLength;
    private readonly ISequenceSource<int> _numbersSource;
    private readonly IStringPartWriter _stringPartWriter;
    private readonly ISequenceSource<bool> _entropySource;

    internal OptimizedLinesGenerator(IStringPartWriter stringPartWriter, ISequenceSource<int>? numbersSource = null, ISequenceSource<bool>? entropySource = null)
    {
        _stringPartWriter = stringPartWriter;
        _numbersSource = numbersSource ?? new RandomPositiveNumberSource();        
        _entropySource = entropySource ?? new EntropySource();
    }

    public OptimizedLinesGenerator(ISequenceSource<string> dictionarySource) : this(new StringPartWriter(dictionarySource)) { }

    public void GenerateLines(long requiredTotalLength, LinesGeneratorHandler lineHandler)
    {
        if (requiredTotalLength < 1)
            throw new ArgumentOutOfRangeException(nameof(requiredTotalLength), "Must be positive");

        long currentLength = 0;

        ReadOnlySpan<char> newlineSpan = Environment.NewLine.AsSpan();
        Span<char> lineBuffer = stackalloc char[MaxLineLength];

        // used for duplicating lines
        int previousStringPartStart = -1;
        int previousLength = -1;

        while (currentLength < requiredTotalLength)
        {
            var nextNumber = _numbersSource.Next();            

            if (!nextNumber.TryFormat(lineBuffer, out int charsWritten, "#\\. "))
                throw new InvalidOperationException($"Can't properly format {nextNumber}");
            var stringPartStart = charsWritten;

            // occasionally if possible the previous line part will be duplicated
            if ((stringPartStart <= previousStringPartStart) && _entropySource.Next())
            {
                lineBuffer[previousStringPartStart..previousLength].CopyTo(lineBuffer[stringPartStart..]);
                charsWritten += previousLength - previousStringPartStart;
            }
            else
            {   
                // provide a new string part
                charsWritten += _stringPartWriter.WriteStringPart(lineBuffer[charsWritten..^newlineSpan.Length]);
                newlineSpan.CopyTo(lineBuffer[charsWritten..]);
                charsWritten += newlineSpan.Length;
            }

            lineHandler(lineBuffer[..charsWritten]);
            
            currentLength += charsWritten;

            previousLength = charsWritten;
            previousStringPartStart = stringPartStart;
        }
    }
}

public delegate void LinesGeneratorHandler(ReadOnlySpan<char> line);
