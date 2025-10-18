# Sortzilla [![.NET build and test](https://github.com/rumkit/Sortzilla/actions/workflows/dotnet-build-and-test.yml/badge.svg)](https://github.com/rumkit/Sortzilla/actions/workflows/dotnet-build-and-test.yml)

<img align="right" src=".github/docs/sortzilla.png" width="250">

- [Description](#description)
- [Build](#build)
- [Usage](#usage)
- [Benchmarks](#benchmarks)


# Description

Sortzilla is a CLI tool that is able to generate text files of a specific format and sort them. The tool supports large files (above 100GB).

The format is a simple text with each line containing number part and text part, e.g. 

```
99. First line of text
42. Another line of text
...

1675. Many-many more lines of text
```

Sorting algorithm considers text part and only if the text of both lines is the same then numbers will be compared

# Build

Checkout/download the repository and run the following command from within the solution folder

```
dotnet build -c Release
```

Default output directory (Sortzilla.CLI\bin\Release\net8.0\) will contain all binaries

# Usage

The CLI tool provides embedded commands help and examples

```
>>.\Sortzilla.CLI.exe --help

USAGE:
    Sortzilla.CLI.dll [OPTIONS] <COMMAND>

EXAMPLES:
    Sortzilla.CLI.dll generate output.txt --size 10G
    Sortzilla.CLI.dll gen output.txt -s 10K -d dictionary.txt
    Sortzilla.CLI.dll valid file.txt
    Sortzilla.CLI.dll sort file.txt
    Sortzilla.CLI.dll sort input.txt -o input-sorted.txt -w 16 -t .\TMP

OPTIONS:
    -h, --help    Prints help information

COMMANDS:
    generate <FileName>    Generates a new file
    validate <FileName>    Validates existing file and checks whether it is sorted
    sort <FileName>        Sorts the provided file
```

# Benchmarks

## Rig

- CPU: i9-13980HX (2.20 GHz) (8 performance cores + 16 efficiency cores)
- RAM: 32GB
- Disk: External USB SSD capped at 10Gb/s

## Memory consumption

Typical memory consumption for generation is ~50MB and for validation is ~30MB. Both generation and validation are single threaded operations and their memory consumption does not depend on the file size.

For sorting operation memory consumption depends on the amount of worker threads and peaks at approximately `Nworkers * 1.5 * chunkSize`. Thus, a sorting job with 16 worker threads using default chunks of 128MB will peak at ~3GB with some overhead value. 

## Timings

- 100GB file generation with 10K words dictionary ~7 minutes

`.\Sortzilla.CLI.exe generate output.txt -s 100G -d .\english-10k-sorted.txt`

- 100GB file validation ~6.5 minutes

`.\Sortzilla.CLI.exe valid output.txt`

- Sorting
  - 1GB file    ~20 seconds
  - 10GB file   ~4.5 minutes
  - 100GB file   > 100 minutes

`.\Sortzilla.CLI.exe sort output.txt -w 16`