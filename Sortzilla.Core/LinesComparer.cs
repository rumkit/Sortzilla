using System.Text.RegularExpressions;

namespace Sortzilla.Core;

internal class LinesComparer : IComparer<string>
{
    public int Compare(string? left, string? right)
    {
        if(left == null)
            throw new ArgumentException(nameof(left), "Cannot be null");
        if (right == null)
            throw new ArgumentException(nameof(right), "Cannot be null");

        var regex = LineTools.LineRegex();

        var matchLeft = regex.Match(left);
        if(!matchLeft.Success)
            throw new ArgumentException(nameof(left), "Line format is invalid");
        var matchRight = regex.Match(right);
        if (!matchRight.Success)
            throw new ArgumentException(nameof(right), "Line format is invalid");

        var stringComparisonResult = string.Compare(
            matchLeft.Groups["string"].Value,
            matchRight.Groups["string"].Value,
            StringComparison.Ordinal);

        if(stringComparisonResult != 0)
            return stringComparisonResult;

        var leftNumber = int.Parse(matchLeft.Groups["number"].Value);
        var rightNumber = int.Parse(matchRight.Groups["number"].Value);

        return leftNumber.CompareTo(rightNumber);
    }
}

public static partial class LineTools
{
    [GeneratedRegex(@"^(?'number'\d+)\.\s(?'string'[a-zA-z\s]*)$", RegexOptions.Compiled)]
    public static partial Regex LineRegex();
}
