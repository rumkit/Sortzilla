namespace Sortzilla.Core.Generator;

public class SimpleLinesGenerator(ISequenceSource<int> digitsSource, ISequenceSource<string> wordsSource)
{
    public IEnumerable<string> GenerateLines(long requiredLength)
    {
        if(requiredLength < 1)
            throw new ArgumentOutOfRangeException(nameof(requiredLength), "Must be positive");

        long currentLength = 0;

        while(currentLength < requiredLength)
        {
            var nextNumber = digitsSource.Next();
            var nextString = wordsSource.Next();
            var nextLine = $"{nextNumber}. {nextString}{Environment.NewLine}";
            currentLength += nextLine.Length;
            yield return nextLine;
        }
    }
}
