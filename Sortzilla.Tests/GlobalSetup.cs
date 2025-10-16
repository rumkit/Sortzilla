[assembly: Retry(3)]
[assembly: System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]

namespace Sortzilla.Tests;

public class GlobalHooks
{
    // Integration tests cleanup
    public const string TestFileName = "testfile.txt";
    public const string OutputFileName = "testfile-sorted.txt";

    [After(TestSession)]
    public static void CleanUp()
    {
        if(File.Exists(TestFileName))
            File.Delete(TestFileName);

        if(File.Exists(OutputFileName))
            File.Delete(OutputFileName);
    }
}
