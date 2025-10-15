namespace Sortzilla.Core.Sorter;

internal class SortSettings
{
    internal const int MaxLineLength = 100;
    public string TempPath { get; init; } = Path.Combine(Path.GetTempPath(), "Sortzilla");
    public int ChunkSizeBytes { get; init; } = 256 * 1024 * 1024; // 256 MB
    public int MaxWorkersCount { get; init; } = Environment.ProcessorCount;
}