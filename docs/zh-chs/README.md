# 命令行解析

[English][en]|[简体中文][zh-chs]|[繁體中文][zh-cht]
-|-|-|-

[en]: /README.md
[zh-chs]: /docs/zh-chs/README.md
[zh-cht]: /docs/zh-cht/README.md

dotnetCampus.CommandLine 中提供了简单而高性能的命令行解析功能，在 dotnetCampus.Cli 命名空间下。

## 快速使用

```csharp
class Program
{
    static void Main(string[] args)
    {
        // 从命令行参数创建一个 CommandLine 类型的新实例。
        var commandLine = CommandLine.Parse(args);

        // 将 CommandLine 类型的当作一个命令行参数类型 Options 来使用。
        var options = commandLine.As<Options>(new OptionsParser());

        // 接下来，使用你的 options 对象编写其他的功能。
    }
}
```

而这个 `Options` 类型的定义如下：

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

于是，在命令行中可以使用不同风格的命令填充这个类型的实例。

Windows 风格：

```powershell
> demo.exe "C:\Users\lvyi\Desktop\demo.txt" -s -Mode Edit -StartupSessions A B C
```

```cmd
> demo.exe "C:\Users\lvyi\Desktop\demo.txt" /s /Mode Edit /StartupSessions A B C
```

Linux 风格：

```bash
$ demo.exe "C:/Users/lvyi/Desktop/demo.txt" -s --mode Edit --startup-sessions A B C
```

以上不同风格不能混合使用。

对于 bool 类型的属性，在命令行中既可以在选项后传入 `true` / `True` / `false` / `False` 也可以不传。如果不传，则表示 `true`。

另外，`ValueAttribute` 和 `OptionAttribute` 可以出现在同一个属性上。这时如果发现了不带选项的值，将填充到 `ValueAttribute` 的属性上；而一旦之后发现了此 `OptionsAttribute` 指定的短名称或者长名称，会将新的值覆盖再次设置到此属性上。

```csharp
[Value(0), Option('f', "File")]
public string FilePath { get; }
```

## 需要注意

在命令行中输入参数时，无论哪种风格，命令行都是区分大小写的。对于选项（`-`、`/` 或者 `--` 开头）如果大小写错误，此选项和后面附带的值都将被忽略；对于值（不带 `-` 或者 `/` 开头），值将按照命令行中的原生大小写传入 `Options` 类型的实例中。

在 `Options` 类型中定义属性时，短名称是可选指定的，但一旦指定则必须是一个字符；长名称是必须指定的，而且命名必须满足 PascalCase 命名规则且不带连字符。详细要求可在编写自己的 `Options` 类型时阅读 `OptionAttribute` 的注释。

## 多种命令行参数与谓词

你可以为你的命令行参数指定谓词，每一种谓词都可以有自己的一组独特的参数类型。

```csharp
var commandLine = CommandLine.Parse(args);
commandLine.AddHandler<EditOptions>(options => 0)
    .AddHandler<PrintOptions>(options => 0).Run();
```

而 `EditOptions` 和 `PrintOptions` 的定义如下，区别在于类型标记了谓词。

```csharp
[Verb("Edit")]
public class EditOptions
{
    [Value(0), Option('f', "File")] public string FilePath { get; set; }
}

[Verb("Print")]
public class PrintOptions
{
    [Value(0), Option('f', "File")] public string FilePath { get; set; }
    [Option('p', "Printer")] public string Printer { get; set; }
}
```

你也可以在 `Handle` 中使用不标谓词的参数类型，但是这样的参数最多只允许有一个，会作为没有任何谓词匹配上时使用的默认参数类型。

另外，`Handle` 方法有对应的 `HandleAsync` 异步版本，用于处理异步的任务。

## 关于解析器

在前面的示例程序中，我们传入了一个解析器的实例 `new OptionsParser()`。如果你不在乎性能（实际上也花不了多少性能），那么不必传入，命令行解析器可以针对你的选项类型自动生成一个运行时解析器。但是如果你在乎性能，那么你可能需要自己编写（将来会自动生成）。本文文末附有各种不同用法的性能数据。

如果你打算自己编写了，那么继续阅读这一小节。

这个类的编写是标准化的，请按照注释以及下文的规范来操作，不要编写额外的任何校验代码或类型转换代码。因为传入解析器之前所有参数已全部校验完毕；而且将来接入自动生成解析器后，你编写的个性化逻辑可能丢失。

解析器的示例代码有两种，一种使用原生接口，性能更好；另一种使用基类，但编写所需的代码更少。建议手写的话使用基类，如果能生成代码，则使用接口。

```csharp
public class OptionsParser : ICommandLineOptionParser<Options>
{
    private string _filePath;
    private bool _isSilence;
    private string _startMode;
    private IReadonlyList<string> _startupSessions;

    public void SetValue(int index, string value)
    {
        switch (index)
        {
            case 0:
                _filePath = value;
                break;
        }
    }

    public void SetValue(char shortName, bool value)
    {
        switch (shortName)
        {
            case 's':
                _isSilence = value;
                break;
        }
    }

    public void SetValue(char shortName, string value)
    {
        switch (shortName)
        {
            case 'm':
                _startMode = value;
                break;
        }
    }

    public void SetValue(char shortName, IReadOnlyList<string> values)
    {
    }

    public void SetValue(string longName, bool value)
    {
        switch (longName)
        {
            case "Silence":
                _isSilence = value;
                break;
        }
    }

    public void SetValue(string longName, string value)
    {
        switch (longName)
        {
            case "Mode":
                _startMode = value;
                break;
        }
    }

    public void SetValue(string longName, IReadOnlyList<string> values)
    {
        switch (longName)
        {
            case "StartupSession":
                _startupSession = value;
                break;
        }
    }

    public Options Commit()
    {
        return new Options(_filePath, _isSilence, _startMode, _startupSession);
    }
}
```

```csharp
public class SelfWrittenShareOptionsParser : CommandLineOptionParser<Options>
{
    public SelfWrittenShareOptionsParser()
    {
        string filePath = null;
        bool isSilence = false;
        string startMode = null;
        IReadonlyList<string> startupSessions = null;

        AddMatch(0, value => filePath = value);
        AddMatch('s', value => isSilence = value);
        AddMatch("Silence", value => isSilence = value);
        AddMatch('m', value => startMode = value);
        AddMatch("Mode", value => startMode = value);
        AddMatch("StartupSessions", value => startupSessions = value);

        SetResult(() => new Options(filePath, isSilence, startMode, startupSession));
    }
}
```

## 支持协议解析

`CommandLine` 支持解析 URL 协议中的字符串。

```
@"dotnetCampus://open/?file=C:\Users\lvyi\Desktop\%E9%87%8D%E5%91%BD%E5%90%8D%E8%AF%95%E9%AA%8C.enbx&mode=Display&silence=true&startupSessions=89EA9D26-6464-4E71-BD04-AA6516063D83"
```

请注意，解析 URL 有如下限制：

1. 你的 `Options` 类型中所有的 `ValueAttribute` 都将无法被赋值；
1. 你的 `Options` 类型中不允许出现标记有 `OptionAttribute` 的字符串集合属性。

另外，URL 解析中的选项名称也是大小写敏感的。当你在 `Options` 类型中正确使用 PascalCase 风格定义了选项的长名称后，你在 URL 中既可以使用 PascalCase 风格也可以使用 camelCase 风格。

## 性能数据

|                       Method |          Mean |        Error |       StdDev |  Ratio | RatioSD |
|----------------------------- |--------------:|-------------:|-------------:|-------:|--------:|
|                  ParseNoArgs |      95.20 ns |     1.828 ns |     1.956 ns |   0.09 |    0.00 |
|              ParseNoArgsAuto |     763.12 ns |    14.702 ns |    19.117 ns |   0.69 |    0.02 |
|                 ParseWindows |   1,116.76 ns |    24.612 ns |    23.022 ns |   1.00 |    0.00 |
|             ParseWindowsAuto |   1,974.78 ns |    37.120 ns |    44.189 ns |   1.76 |    0.06 |
|          ParseWindowsRuntime |  96,378.30 ns | 1,900.205 ns | 2,725.217 ns |  86.40 |    2.99 |
| ParseWindowsImmutableRuntime |  96,200.14 ns | 1,677.293 ns | 1,568.941 ns |  86.17 |    2.15 |
|                  HandleVerbs |   1,530.32 ns |    33.916 ns |    31.725 ns |   1.37 |    0.05 |
|           HandleVerbsRuntime |  26,888.69 ns |   660.595 ns |   734.250 ns |  24.18 |    0.71 |
|                     ParseCmd |   1,153.53 ns |    26.479 ns |    27.192 ns |   1.04 |    0.03 |
|                 ParseCmdAuto |   1,915.15 ns |    21.508 ns |    17.960 ns |   1.71 |    0.04 |
|                   ParseLinux |   1,763.82 ns |    33.752 ns |    43.887 ns |   1.58 |    0.05 |
|               ParseLinuxAuto |   2,556.28 ns |    47.460 ns |    42.072 ns |   2.29 |    0.06 |
|                     ParseUrl |   4,800.81 ns |    86.862 ns |    72.534 ns |   4.29 |    0.07 |
|                 ParseUrlAuto |   6,274.80 ns |   125.106 ns |   205.553 ns |   5.65 |    0.25 |
|            CommandLineParser | 136,090.91 ns | 1,072.509 ns |   895.594 ns | 121.71 |    2.66 |

总结来说：完成一次解析只需要 1091ns，也就是大约 10 tick。

说明：

- NoArgs 表示没有传入参数
- Auto 表示自动查找 Parser 而不是手动传入
- Runtime 表示使用运行时解析器
- Handle 表示进行多谓词匹配
- CommandLineParser 是使用的 CommandLineParser 库作为对照
- 测试使用的参数：
    - Windows 风格：`"C:\Users\lvyi\Desktop\重命名试验.enbx" -Cloud -Iwb -m Display -s -p Outside -StartupSession 89EA9D26-6464-4E71-BD04-AA6516063D83`
    - Cmd 风格：`"C:\Users\lvyi\Desktop\重命名试验.enbx" /Cloud /Iwb /m Display /s /p Outside /StartupSession 89EA9D26-6464-4E71-BD04-AA6516063D83`
    - Linux 风格：`"C:\Users\lvyi\Desktop\重命名试验.enbx" --cloud --iwb -m Display -s -p Outside --startup-session 89EA9D26-6464-4E71-BD04-AA6516063D83`
    - Url 风格：`walterlv://open/?file=C:\Users\lvyi\Desktop\%E9%87%8D%E5%91%BD%E5%90%8D%E8%AF%95%E9%AA%8C.enbx&cloud=true&iwb=true&silence=true&placement=Outside&startupSession=89EA9D26-6464-4E71-BD04-AA6516063D83`
