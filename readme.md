# SumType
[Discriminated union][tagged-union] types often found in functional programming languages such as Haskell.

# Description
Handle null through a proper type system that excludes it so you don’t have to keep checking.
Instead of throwing exceptions, return them as values that short-circuit further evaluation.
Let your functions return alternative data types.
This library provides types and LINQ-inspired methods to handle such types.

# Requirements
C♯ 8.0 with [nullable reference types][nullable] enabled, e.g.,
```xml
<NullableContextOptions>enable</NullableContextOptions>
```
in the `.csproj` project file.

# Installation
To add [the package][nuget] to a project, change directory to the project, and
```powershell
dotnet add package LMMarsano.SumType
```

# Development
Install at least [.NET Core 3.0][dotnet], clone the repository, change into the directory, and build.
```PowerShell
git clone https://github.com/lmmarsano/sumtype.git
Set-Location -Path sumtype
dotnet build
```
Edit sources in `source\SumType`.
For [Visual Studio Code][vscode]
```PowerShell
code source\SumType
```

[tagged-union]: https://en.wikipedia.org/wiki/Tagged_union
[nullable]: https://docs.microsoft.com/en-us/dotnet/csharp/nullable-references
[nuget]: https://www.nuget.org/packages/LMMarsano.SumType/
[dotnet]: https://dotnet.microsoft.com/download/dotnet-core/3.0
[vscode]: https://code.visualstudio.com/
