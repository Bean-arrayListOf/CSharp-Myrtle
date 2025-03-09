using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
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
            list.Add(b.ToString("X2"));
        }

        return list;
    }
}