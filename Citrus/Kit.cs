using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace CSharp_Myrtle.Citrus;

/**
 *  工具
 */
public static class Kit
{
    /**
     * 系统类型
     */
    public static readonly OSType Platform = getOSType();

    /**
     * 拓展打印
     */
    public static void Out(this object? any) => Console.Write(any);

    /**
   * 拓展打印
   */
    public static void OutLine(this object? any) => Console.WriteLine(any);

    /**
   * 拓展打印
   */
    public static void Write(this object? any) => Console.Write(any);

    /**
   * 拓展打印
   */
    public static void WriteLine(this object? any) => Console.WriteLine(any);

    public static int? InInt(string? title = null)
    {
        if (title != null)
        {
            Console.Write(title);
        }

        return Console.Read();
    }

    public static double? InDouble(string? title = null)
    {
        if (title != null)
        {
            Console.Write(title);
        }

        return Console.ReadLine()?.ToDouble();
    }

    public static string? InString(string? title = null)
    {
        if (title != null)
        {
            Console.Write(title);
        }

        return Console.ReadLine();
    }

    public static float? InFloat(string? title = null)
    {
        if (title != null)
        {
            Console.Write(title);
        }

        return Console.ReadLine()?.ToFloat();
    }

    public static short? InShoat(string? title = null)
    {
        if (title != null)
        {
            Console.Write(title);
        }

        return Console.ReadLine()?.ToShort();
    }

    public static bool? InBoolean(string? title = null)
    {
        if (title != null)
        {
            Console.Write(title);
        }

        return Console.ReadLine()?.ToBoolean();
    }

    public static long? InLong(string? title = null)
    {
        if (title != null)
        {
            Console.Write(title);
        }

        return Console.ReadLine()?.ToLong();
    }

    public static ushort? InUShort(string? title = null)
    {
        if (title != null)
        {
            Console.Write(title);
        }

        return Console.ReadLine()?.ToUShort();
    }

    public static uint? InUInt(string? title = null)
    {
        if (title != null)
        {
            Console.Write(title);
        }

        return Console.ReadLine()?.ToUInt();
    }

    public static ulong? InULong(string? title = null)
    {
        if (title != null)
        {
            Console.Write(title);
        }

        return Console.ReadLine()?.ToULong();
    }


    /**
     * 创建读取流
     */
    public static TextReader ReaderOf(string file)
    {
        if (!File.Exists(file))
        {
            throw new FileNotFoundException($"file not found {file}");
        }

        if (Directory.Exists(file))
        {
            throw new IOException("file is dir");
        }

        return new StreamReader(File.OpenRead(file));
    }

    /**
     * 读取内容
     */
    public static String ReadText(this Stream stream) => stream.ReadAllByte().EncString();

    /**
     * 读取内容
     *
     */
    public static List<string> ReadLines(this Stream stream)
    {
        var lines = new List<String>();
        var read = new StreamReader(stream);
        while (read.ReadLine() is { } line)
        {
            lines.Add(line);
        }

        return lines;
    }

    /**
     * 读取所有byte
     */
    public static byte[] ReadAllByte(this Stream stream)
    {
        using var baos = new MemoryStream();
        var bytes = new byte[1024];
        var n = 0;
        while ((n = stream.Read(bytes)) > 0)
        {
            baos.Write(bytes, 0, n);
        }

        baos.Flush();
        return baos.ToArray();
    }

    public static byte[] EncBytes(this string hex) => Encoding.Default.GetBytes(hex);

    public static string EncString(this byte[] bytes) => Encoding.Default.GetString(bytes);

    public static ResourceManager ResourceOf(string baseName) => new ResourceManager(baseName, typeof(Kit).Assembly);

    public static ResourceManager GetResource(this Assembly baseAssembly, string baseName) =>
        new ResourceManager(baseName, baseAssembly);

    public static bool ToBoolean(this object? any) => Convert.ToBoolean(any);

    public static short ToShort(this object? any) => Convert.ToInt16(any);

    public static ushort ToUShort(this object? any) => Convert.ToUInt16(any);

    public static int ToInt(this object? any) => Convert.ToInt32(any);

    public static uint ToUInt(this object? any) => Convert.ToUInt32(any);

    public static long ToLong(this object? any) => Convert.ToInt64(any);

    public static ulong ToULong(this object? any) => Convert.ToUInt64(any);

    public static string? ToStrings(this object? any) => Convert.ToString(any);

    public static double ToDouble(this object? any) => Convert.ToDouble(any);

    public static float ToFloat(this object? any) => Convert.ToSingle(any);

    public static OSType getOSType()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return OSType.MacOS;
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return OSType.Linux;
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return OSType.Windows;
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
        {
            return OSType.FreeBSD;
        }

        throw new Exception("未知系统");
    }

    public static ResourceManager GetResource(this Type baseType, string baseName) =>
        new ResourceManager(baseName, baseType.Assembly);

    public static string? Input() => Console.ReadLine();

    public static string HexString(this byte[] data) => String.Join("", data.HexsString());

    public static List<String> HexsString(this byte[] data)
    {
        var list = new List<String>();
        foreach (var b in data)
        {
            list.Add(b.ToString(Env.cr.GetString("Citrus_Kit_HexsString_1")));
        }

        return list;
    }

    public static string GetUUIDv3() => Guid.NewGuid().ToString();

    public static object NeoInstance(this Type type)
    {
        return Activator.CreateInstance(type)!;
    }

    public static void NeoMethod(Type type, object instance, string method, params object?[]? args)
    {
        var info = type.GetMethod(method, FormatType(args));
        info!.Invoke(instance, args);
    }

    public static Type[] FormatType(params object?[]? args)
    {
        var type = new List<Type?>();
        if (args == null)
        {
            type.Add(null);
            return type.ToArray()!;
        }

        foreach (var arg in args)
        {
            if (arg == null)
            {
                type.Add(null);
                continue;
            }

            type.Add(arg.GetType());
        }

        return type.ToArray()!;
    }

    public static byte[] RandomBytes(int? lenget = null)
    {
        var buffer = new byte[lenget ?? Env.randomByteLength];
        using var kit = RandomNumberGenerator.Create();
        kit.GetBytes(buffer);
        return buffer;
    }

    public static byte[] RandomHash(this HashMode sha) => Hash.Get(sha, RandomBytes());

    public static string RandomHashHex(this HashMode sha) => Hash.Get(sha, RandomBytes()).HexString();

    public static string PrintTable(List<string> title, List<List<string>> data)
    {
        // 检查标题和数据是否为空
        if (title == null || data == null || data.Count == 0)
        {
            throw new Exception("标题或数据为空！");
        }

        // 计算每列的最大宽度
        List<int> columnWidths = new List<int>();
        for (int i = 0; i < title.Count; i++)
        {
            int maxWidth = GetStringWidth(title[i]);
            foreach (var row in data)
            {
                int rowWidth = GetStringWidth(row[i]);
                if (rowWidth > maxWidth)
                {
                    maxWidth = rowWidth;
                }
            }

            columnWidths.Add(maxWidth + 2); // 加 2 是为了留出一些空白
        }

        var buffer = new StringBuilder();
        // 打印表格顶部边框
        buffer.Append(PrintBorder(columnWidths));

        // 打印标题行
        buffer.Append(PrintRow(title, columnWidths, true));

        // 打印标题和数据之间的分隔线
        buffer.Append(PrintBorder(columnWidths));

        // 打印数据行
        foreach (var row in data)
        {
            buffer.Append(PrintRow(row, columnWidths, false));
        }

        // 打印表格底部边框
        buffer.Append(PrintBorder(columnWidths));

        return buffer.ToString();
    }

    static string PrintBorder(List<int> columnWidths)
    {
        var buffer = new StringBuilder();
        buffer.Append("+");
        foreach (int width in columnWidths)
        {
            buffer.Append(new string('-', width) + "+");
        }

        buffer.AppendLine();
        return buffer.ToString();
    }

    static string PrintRow(List<string> row, List<int> columnWidths, bool isHeader)
    {
        var buffer = new StringBuilder();
        buffer.Append("|");
        for (int i = 0; i < row.Count; i++)
        {
            // 计算内容宽度
            int contentWidth = GetStringWidth(row[i]);
            int padding = columnWidths[i] - contentWidth;

            // 左对齐
            string content = row[i].PadRight(row[i].Length + padding);
            buffer.Append(content + "|");
        }

        buffer.AppendLine();
        return buffer.ToString();
    }

    // 计算字符串的显示宽度（中文占 2 个字符，英文占 1 个字符）
    static int GetStringWidth(string str)
    {
        int width = 0;
        foreach (char c in str)
        {
            width += (c >= 0x4E00 && c <= 0x9FA5) ? 2 : 1; // 判断是否为中文字符
        }

        return width;
    }
}