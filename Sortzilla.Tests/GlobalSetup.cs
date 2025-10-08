// Here you could define global logic that would affect all tests

// You can use attributes at the assembly level to apply to all tests in the assembly
[assembly: Retry(3)]
[assembly: System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]

namespace Sortzilla.Tests;

public class GlobalHooks
{
    public const string TestFileName = "testfile.txt";

    [After(TestSession)]
    public static void CleanUp()
    {
        if(File.Exists(TestFileName))
            File.Delete(TestFileName);
    }
}
