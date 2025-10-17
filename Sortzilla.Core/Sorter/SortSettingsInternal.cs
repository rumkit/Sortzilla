namespace Sortzilla.Core.Sorter;

internal class SortSettingsInternal
{
    public const int MaxLineLength = 100;
    public const int DefaultReadBuffer = 4096;
    public const int DefaultWriteBuffer = 10_485_760;

    public required string TempPath { get; init; }
    public required int ChunkSizeBytes { get; init; }
    public required int MaxWorkersCount { get; init; }
}