# dotnetCampus.CommandLine

![Build](https://github.com/dotnet-campus/dotnetCampus.CommandLine/workflows/.NET%20Build/badge.svg)  ![NuGet Package](https://github.com/dotnet-campus/dotnetCampus.CommandLine/workflows/NuGet%20Publish/badge.svg) [![dotnetCamus.CommandLine](https://img.shields.io/nuget/v/dotnetCamus.CommandLine)](https://www.nuget.org/packages/dotnetCamus.CommandLine/)

[English][en]|[简体中文][zh-chs]|[繁體中文][zh-cht]
-|-|-

[en]: /README.md
[zh-chs]: /docs/zh-chs/README.md
[zh-cht]: /docs/zh-cht/README.md

dotnetCampus.CommandLine is probably the fastest command line parser in all .NET open-source projects.

Parsing a classical command line only takes 1091ns, thus 10 ticks.

## Get Started

For your program `Main` method, write this code below:

```csharp
class Program
{
    static void Main(string[] args)
    {
        var commandLine = CommandLine.Parse(args);
        var options = commandLine.As<Options>(new OptionsParser());

        // Then, use your Options instance here.
    }
}
```

You need to define the `Options` class as followed below:

```csharp
class Options
{
    [Value(0)]
    public string FilePath { get; }

    [Option('s', "Silence")]
    public bool IsSilence { get; }

    [Option('m', "Mode")]
    public string StartMode { get; }

    [Option("StartupSessions")]
    public IReadonlyList<string> StartupSessions { get; }

    public Options(
        string filePath,
        bool isSilence,
        string startMode,
        IReadonlyList<string> startupSessions)
    {
        FilePath = filePath;
        IsSilence = isSilence;
        StartMode = startMode;
        StartupSessions = startupSessions;
    }
}
```

Then you can run your program by passing these kind of command line args:

Windows style：

```powershell
> demo.exe "C:\Users\lvyi\Desktop\demo.txt" -s -Mode Edit -StartupSessions A B C
```

```cmd
> demo.exe "C:\Users\lvyi\Desktop\demo.txt" /s /Mode Edit /StartupSessions A B C
```

Linux style：

```bash
$ demo.exe "C:/Users/lvyi/Desktop/demo.txt" -s --mode Edit --startup-sessions A B C
```

Notice that you cannot use different styles in a single command line.

For `bool`:

- You can pass `true` / `True` / `false` / `False` to specify a boolean value;
- You can pass nothing but only a switch.

It means that `-s true`, `-s True`, `-s` are the same.

For `ValueAttribute` and `OptionAttribute`:

- You can specify both on a single property.
- If there is a value without option the property got the value, but if another value with the specified option exists, the new value will override the old one.

```csharp
[Value(0), Option('f', "File")]
public string FilePath { get; }
```

## Engage, Contribute and Provide Feedback

Thank you very much for firing a new issue and providing new pull requests.

### Issue

Click here to file a new issue:

- [New Issue · dotnet-campus/dotnetCampus.CommandLine](https://github.com/dotnet-campus/dotnetCampus.CommandLine/issues/new)

### Contributing Guide

Be kindly.

## License

dotnetCampus.CommandLine is licensed under the [MIT license](/LICENSE).
