namespace Sortzilla.Tests.TestUtils;

internal class FileContentsValidator
{
    private readonly Dictionary<string, int> _linesDictionary = new ();

    public void CollectData(string fileName)
    {
        var lines = File.ReadAllLines(fileName);
        foreach (var line in lines)
        {
            if (!_linesDictionary.TryGetValue(line, out var value))
            {
                value = 0;
            }

            _linesDictionary[line] = ++value;
        }
    }

    public bool Validate(string fileName)
    {
        var lines = File.ReadAllLines(fileName);
        foreach (var line in lines)
        {
            if (!_linesDictionary.TryGetValue(line, out var value))
            {
                return false;
            }

            if(value > 1)
            {
                _linesDictionary[line] = --value;
            }
            else
            {
                _linesDictionary.Remove(line);
            }
        }

        return _linesDictionary.Count == 0;
    }
}
