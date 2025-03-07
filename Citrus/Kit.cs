using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace CSharp_Myrtle.Citrus;

public static class Kit
{
    public static readonly OSType Platform = getOSType();

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void Out(this object? any) => Console.Write(any);

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void OutLine(this object? any) => Console.WriteLine(any);

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void Write(this object? any) => Console.Write(any);

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void WriteLine(this object? any) => Console.WriteLine(any);

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

    public static String ReadText(this Stream stream)
    {
        using (var baos = new MemoryStream())
        {
            var bytes = new byte[1024];
            var n = 0;
            while((n = stream.Read(bytes)) > 0)
            {
                baos.Write(bytes, 0, n);
            }
            baos.Flush();
            return baos.ToArray().EncString();
        }
    }

    public static List<string> ReadLines(this Stream stream)
    {
        var lines = new List<String>();
        var read = new StreamReader(stream);
        while(read.ReadLine() is { } line)
        {
            lines.Add(line);
        }
        return lines;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static byte[] EncBytes(this string hex) => Encoding.Default.GetBytes(hex);

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static string EncString(this byte[] bytes) => Encoding.Default.GetString(bytes);

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static ResourceManager ResourceOf(string baseName) => new ResourceManager(baseName, typeof(Kit).Assembly);

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static ResourceManager GetResource(this Assembly baseAssembly, string baseName) => new ResourceManager(baseName, baseAssembly);

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static bool ToBoolean(this object? any) => Convert.ToBoolean(any);

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static short ToShort(this object? any) => Convert.ToInt16(any);

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static ushort ToUShort(this object? any) => Convert.ToUInt16(any);

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static int ToInt(this object? any) => Convert.ToInt32(any);

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static uint ToUInt(this object? any) => Convert.ToUInt32(any);

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static long ToLong(this object? any) => Convert.ToInt64(any);

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static ulong ToULong(this object? any) => Convert.ToUInt64(any);

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static string? ToStrings(this object? any) => Convert.ToString(any);

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static double ToDouble(this object? any) => Convert.ToDouble(any);

    [MethodImpl(MethodImplOptions.NoInlining)]
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

    public static ResourceManager GetResource(this Type baseType,string baseName)
    {
        return new ResourceManager(baseName, baseType.Assembly);
    }
}