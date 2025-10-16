namespace Sortzilla.Core.Sorter;

internal class SortSettingsInternal
{
    public const int MaxLineLength = 100;
    public required string TempPath { get; init; }
    public required int ChunkSizeBytes { get; init; }
    public required int MaxWorkersCount { get; init; }
}