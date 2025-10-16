namespace Sortzilla.Core.Sorter;

public class SortSettings
{
    public string? TempPath { get; init; } 
    public int? ChunkSizeBytes { get; init; }
    public int? MaxWorkersCount { get; init; }
}
