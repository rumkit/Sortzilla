# Sortzilla [![.NET build and test](https://github.com/rumkit/Sortzilla/actions/workflows/dotnet-build-and-test.yml/badge.svg)](https://github.com/rumkit/Sortzilla/actions/workflows/dotnet-build-and-test.yml)

<img align="right" src=".github/docs/sortzilla.png" width="400">

- [Description](#description)
- [Build](#build)
- [Usage](#usage)


# Description

Sortzilla is a CLI tool that is able to generate text files of a specefic format and sort them. The tool supports large files (above 100GB).

The format is a simple text with each line containing number part and text part, e.g. 

```
99. First line of text
42. Another line of text
...

1675. Many-many more lines of text
```

Sorting algorithm considers text part and only if the text of both lines is the same then numbers will be compared

# Build

Checkout/download the repository and the following command from within the solution folder

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