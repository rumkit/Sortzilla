namespace Sortzilla.Core;

internal class OptimizedLinesComparer : IComparer<string>
{
    public int Compare(string? left, string? right)
    {
        if(left == null)
            throw new ArgumentException("Cannot be null", nameof(left));
        if (right == null)
            throw new ArgumentException("Cannot be null", nameof(right));

        var leftSpan = left.AsSpan();
        var rightSpan = right.AsSpan();

        var leftDotIndex = left.IndexOf('.');
        if (leftDotIndex <= 0 )
            throw new ArgumentException("Line format is invalid", nameof(left));
        
        var rightDotIndex = rightSpan.IndexOf('.');
        if (rightDotIndex <= 0)
            throw new ArgumentException("Line format is invalid", nameof(right));       

        var stringPartComparisonResult = leftSpan[leftDotIndex ..].CompareTo(rightSpan[rightDotIndex ..], StringComparison.Ordinal);

        if(stringPartComparisonResult != 0)
            return stringPartComparisonResult;

        if (int.TryParse(leftSpan[.. leftDotIndex], out var leftNumber)
            && int.TryParse(rightSpan[.. rightDotIndex], out var rightNumber))
        {
            return leftNumber.CompareTo(rightNumber);
        }

        throw new ArgumentException("Line format is invalid", "numbers");
    }
}
