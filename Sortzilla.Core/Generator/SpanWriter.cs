namespace Sortzilla.Core.Generator;

public class SpanWriter
{
    private readonly Random _random = new ();
    private const string _chars = "abcdefghijklmnopqrstuvwxyz";

    private char GetNextChar() => _chars[_random.Next(_chars.Length)];
    public void Write(string fileName, long requiredLength)
    {
        if (requiredLength < 1)
            throw new ArgumentOutOfRangeException(nameof(requiredLength), "Must be positive");

        using var fStream = File.Create(fileName);
        using var writer = new StreamWriter(fStream, bufferSize: 10_000_000);

        long currentLength = 0;
        Span<char> numberSpan = stackalloc char[11];
        Span<char> resultSpan = stackalloc char[100];
        var newLine = Environment.NewLine.ToCharArray().AsSpan();

        while (currentLength < requiredLength)
        {
            var number = _random.Next(1, int.MaxValue);
            number.TryFormat(numberSpan, out int charsWritten);
            numberSpan.CopyTo(resultSpan);
            var resultPointer = charsWritten;
            resultSpan[resultPointer++] = '.';
            resultSpan[resultPointer++] = ' ';

            while(resultPointer < resultSpan.Length - newLine.Length)
            {
                resultSpan[resultPointer++] = GetNextChar();
            }

            newLine.CopyTo(resultSpan[resultPointer ..]);
            resultPointer += newLine.Length;
            writer.Write(resultSpan);
            currentLength += resultPointer;
        }

        writer.Flush();
    }
}
