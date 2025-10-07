namespace Sortzilla.Core.Generator;

public class SimpleLinesGenerator(ISequenceSource<int> digitsSource, ISequenceSource<string> wordsSource)
{
    private ISequenceSource<int> _digitsSource = digitsSource;
    private ISequenceSource<string> _wordsSource = wordsSource;

    public IEnumerable<string> GenerateLines(long requiredLength)
    {
        if(requiredLength < 1)
            throw new ArgumentOutOfRangeException(nameof(requiredLength), "Must be positive");

        long currentLength = 0;

        while(currentLength < requiredLength)
        {
            var nextNumber = _digitsSource.Next();
            var nextString = _wordsSource.Next();
            var nextLine = $"{nextNumber}. {nextString}{Environment.NewLine}";
            currentLength += nextLine.Length;
            yield return nextLine;
        }
    }
}
