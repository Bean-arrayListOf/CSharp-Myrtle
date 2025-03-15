using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
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

	public static bool FileExists([NotNullWhen(true)] this string file) => File.Exists(file);

	public static FileStream FileCreate(this string file) => File.Create(file);

	public static StreamWriter FileCreateText(this string file) => File.CreateText(file);

	public static FileSystemInfo FileSystemInfo(this string file, string pathToTarget) =>
		File.CreateSymbolicLink(file, pathToTarget);

	public static void FileDelete(this string file) => File.Delete(file);

	public static string FileReadAllText(this string file) => File.ReadAllText(file);

	public static string FileReadAllText(this string file, Encoding enc) => File.ReadAllText(file, enc);

	public static void FileCopy(this string sourceFileName, string destFileName) =>
		File.Copy(sourceFileName, destFileName);

	public static void FileCopy(this string sourceFileName, string destFileName, bool overwrite) =>
		File.Copy(sourceFileName, destFileName, overwrite);

	public static FileStream FileCreate(this string path, int bufferSize) => File.Create(path, bufferSize);

	public static FileStream FileCreate(this string path, int bufferSize, FileOptions options) =>
		File.Create(path, bufferSize, options);

	public static FileStream FileOpen(this string path, FileStreamOptions options) => File.Open(path, options);

	public static FileStream FileOpen(this string path, FileMode mode) => File.Open(path, mode);

	public static FileStream FileOpen(this string path, FileMode mode, FileAccess access) =>
		File.Open(path, mode, access);

	public static FileStream FileOpen(this string path, FileMode mode, FileAccess access, FileShare share) =>
		File.Open(path, mode, access, share);

	public static void FileSetCreationTime(this string path, DateTime creationTime) =>
		File.SetCreationTime(path, creationTime);

	public static void FileSetCreationTimeUtc(this string path, DateTime creationTimeUtc) =>
		File.SetCreationTimeUtc(path, creationTimeUtc);

	public static DateTime FileGetCreationTime(this string path) => File.GetCreationTime(path);

	public static DateTime FileGetCreationTimeUtc(this string path) => File.GetCreationTimeUtc(path);

	public static void FileSetLastAccessTime(this string path, DateTime lastAccessTime) =>
		File.SetLastAccessTime(path, lastAccessTime);

	public static void FileSetLastAccessTimeUtc(this string path, DateTime lastAccessTimeUtc) =>
		File.SetLastAccessTimeUtc(path, lastAccessTimeUtc);

	public static DateTime FileGetLastAccessTime(this string path) => File.GetLastAccessTime(path);

	public static DateTime FileGetLastAccessTimeUtc(this string path) => File.GetLastAccessTimeUtc(path);

	public static void FileSetLastWriteTime(this string path, DateTime lastWriteTime) =>
		File.SetLastWriteTime(path, lastWriteTime);

	public static void FileSetLastWriteTimeUtc(this string path, DateTime lastWriteTimeUtc) =>
		File.SetLastWriteTime(path, lastWriteTimeUtc);

	public static DateTime FileGetLastWriteTime(this string path) => File.GetLastWriteTime(path);

	public static DateTime FileGetLastWriteTimeUtc(this string path) => File.GetLastWriteTimeUtc(path);

	public static FileAttributes FileGetAttributes(this string path) => File.GetAttributes(path);

	public static void FileSetAttributes(this string path, FileAttributes fileAttributes) =>
		File.SetAttributes(path, fileAttributes);

	[UnsupportedOSPlatform("windows")]
	public static UnixFileMode FileGetUnixFileMode(this string path) => File.GetUnixFileMode(path);

	[UnsupportedOSPlatform("windows")]
	public static void FileSetUnixFileMode(this string path, UnixFileMode mode) => File.SetUnixFileMode(path, mode);

	public static FileStream FileOpenRead(this string path) => File.OpenRead(path);

	public static FileStream FileOpenWrite(this string path) => File.OpenWrite(path);

	public static void FileWriteAllText(this string path, string? contents) => File.WriteAllText(path, contents);

	public static void FileWriteAllText(this string path, string? contents, Encoding encoding) =>
		File.WriteAllText(path, contents, encoding);

	public static byte[] FileReadAllBytes(this string path) => File.ReadAllBytes(path);

	public static void FileWriteAllBytes(this string path, byte[] bytes) => File.WriteAllBytes(path, bytes);

	public static string[] FileReadAllLines(this string path) => File.ReadAllLines(path);

	public static string[] FileReadAllLines(this string path, Encoding encoding) => File.ReadAllLines(path, encoding);

	public static IEnumerable<string> FileReadLines(this string path) => File.ReadLines(path);

	public static IEnumerable<string> FileReadLines(this string path, Encoding encoding) =>
		File.ReadLines(path, encoding);

	public static IAsyncEnumerable<string> FileReadLinesAsync(this string path,
		CancellationToken cancellationToken = default) => File.ReadLinesAsync(path, cancellationToken);

	public static IAsyncEnumerable<string> FileReadLinesAsync(this string path, Encoding encoding,
		CancellationToken cancellationToken = default) => File.ReadLinesAsync(path, encoding, cancellationToken);

	public static void FileWriteAllLines(this string path, string[] contents) => File.WriteAllLines(path, contents);

	public static void FileWriteAllLines(this string path, IEnumerable<string> contents) =>
		File.WriteAllLines(path, contents);

	public static void FileWriteAllLines(this string path, string[] contents, Encoding encoding) =>
		File.WriteAllLines(path, contents, encoding);

	public static void FileWriteAllLines(this string path, IEnumerable<string> contents, Encoding encoding) =>
		File.WriteAllLines(path, contents, encoding);

	public static void FileAppendAllText(this string path, string? contents) => File.AppendAllText(path, contents);

	public static void FileAppendAllText(this string path, string? contents, Encoding encoding) =>
		File.AppendAllText(path, contents, encoding);

	public static void FileAppendAllLines(this string path, IEnumerable<string> contents) =>
		File.AppendAllLines(path, contents);

	public static void FileAppendAllLines(this string path, IEnumerable<string> contents, Encoding encoding) =>
		File.AppendAllLines(path, contents, encoding);

	public static void FileReplace(this string sourceFileName, string destinationFileName,
		string? destinationBackupFileName) =>
		File.Replace(sourceFileName, destinationFileName, destinationBackupFileName);

	public static void FileReplace(this string sourceFileName, string destinationFileName,
		string? destinationBackupFileName, bool ignoreMetadataErrors) => File.Replace(sourceFileName,
		destinationFileName, destinationBackupFileName, ignoreMetadataErrors);

	public static void FileMove(this string sourceFileName, string destFileName) =>
		File.Move(sourceFileName, destFileName);

	public static void FileMove(this string sourceFileName, string destFileName, bool overwrite) =>
		File.Move(sourceFileName, destFileName, overwrite);

	[SupportedOSPlatform("windows")]
	public static void FileEncrypt(this string path) => File.Encrypt(path);

	[SupportedOSPlatform("windows")]
	public static void FileDecrypt(this string path) => File.Decrypt(path);

	public static Task<string> FileReadAllTextAsync(this string path,
		CancellationToken cancellationToken = default(CancellationToken)) =>
		File.ReadAllTextAsync(path, cancellationToken);

	public static Task<string> FileReadAllTextAsync(this string path, Encoding encoding,
		CancellationToken cancellationToken = default(CancellationToken)) =>
		File.ReadAllTextAsync(path, encoding, cancellationToken);

	public static Task FileWriteAllTextAsync(this string path, string? contents,
		CancellationToken cancellationToken = default(CancellationToken)) =>
		File.WriteAllTextAsync(path, contents, cancellationToken);

	public static Task FileWriteAllTextAsync(this string path, string? contents, Encoding encoding,
		CancellationToken cancellationToken = default(CancellationToken)) =>
		File.WriteAllTextAsync(path, contents, encoding, cancellationToken);

	public static Task<byte[]> FileReadAllBytesAsync(this string path,
		CancellationToken cancellationToken = default(CancellationToken)) =>
		File.ReadAllBytesAsync(path, cancellationToken);

	public static Task FileWriteAllBytesAsync(this string path, byte[] bytes,
		CancellationToken cancellationToken = default(CancellationToken)) =>
		File.ReadAllBytesAsync(path, cancellationToken);

	public static Task<string[]> FileReadAllLinesAsync(this string path,
		CancellationToken cancellationToken = default(CancellationToken)) =>
		File.ReadAllLinesAsync(path, cancellationToken);

	public static Task<string[]> FileReadAllLinesAsync(this string path, Encoding encoding,
		CancellationToken cancellationToken = default(CancellationToken)) =>
		File.ReadAllLinesAsync(path, encoding, cancellationToken);

	public static Task FileWriteAllLinesAsync(this string path, IEnumerable<string> contents,
		CancellationToken cancellationToken = default(CancellationToken)) =>
		File.WriteAllLinesAsync(path, contents, cancellationToken);

	public static Task FileWriteAllLinesAsync(this string path, IEnumerable<string> contents, Encoding encoding,
		CancellationToken cancellationToken = default(CancellationToken)) =>
		File.WriteAllLinesAsync(path, contents, encoding, cancellationToken);

	public static Task FileAppendAllTextAsync(this string path, string? contents,
		CancellationToken cancellationToken = default(CancellationToken)) =>
		File.AppendAllTextAsync(path, contents, cancellationToken);

	public static Task FileAppendAllTextAsync(this string path, string? contents, Encoding encoding,
		CancellationToken cancellationToken = default(CancellationToken)) =>
		File.AppendAllTextAsync(path, contents, encoding, cancellationToken);

	public static Task FileAppendAllLinesAsync(this string path, IEnumerable<string> contents,
		CancellationToken cancellationToken = default(CancellationToken)) =>
		File.AppendAllLinesAsync(path, contents, cancellationToken);

	public static Task FileAppendAllLinesAsync(this string path, IEnumerable<string> contents, Encoding encoding,
		CancellationToken cancellationToken = default(CancellationToken)) =>
		File.AppendAllLinesAsync(path, contents, encoding, cancellationToken);

	public static FileSystemInfo FileCreateSymbolicLink(this string path, string pathToTarget) =>
		File.CreateSymbolicLink(path, pathToTarget);

	public static FileSystemInfo? FileResolveLinkTarget(this string linkPath, bool returnFinalTarget) =>
		File.ResolveLinkTarget(linkPath, returnFinalTarget);

	public static DirectoryInfo? DirectoryGetParent(this string path) => Directory.GetParent(path);

	public static DirectoryInfo DirectoryCreateDirectory(this string path) => Directory.CreateDirectory(path);

	public static DirectoryInfo DirectoryCreateDirectory(this string path, UnixFileMode unixCreateMode) =>
		Directory.CreateDirectory(path, unixCreateMode);

	public static DirectoryInfo DirectoryCreateTempSubdirectory(this string? prefix) =>
		Directory.CreateTempSubdirectory(prefix ?? null);

	public static bool DirectoryExists([NotNullWhen(true)] this string? path) => Directory.Exists(path);

	public static void DirectorySetCreationTime(this string path, DateTime creationTime) =>
		Directory.SetCreationTime(path, creationTime);

	public static void DirectorySetCreationTimeUtc(this string path, DateTime creationTimeUtc) =>
		Directory.SetCreationTimeUtc(path, creationTimeUtc);

	public static DateTime DirectoryGetCreationTime(this string path) => Directory.GetCreationTime(path);

	public static DateTime DirectoryGetCreationTimeUtc(this string path) => Directory.GetCreationTimeUtc(path);

	public static void DirectorySetLastWriteTime(this string path, DateTime lastWriteTime) =>
		Directory.SetLastWriteTime(path, lastWriteTime);

	public static void DirectorySetLastWriteTimeUtc(this string path, DateTime lastWriteTimeUtc) =>
		Directory.SetLastWriteTimeUtc(path, lastWriteTimeUtc);

	public static DateTime DirectoryGetLastWriteTime(this string path) => Directory.GetLastWriteTime(path);

	public static DateTime DirectoryGetLastWriteTimeUtc(this string path) => Directory.GetLastWriteTimeUtc(path);

	public static void DirectorySetLastAccessTime(this string path, DateTime lastAccessTime) =>
		Directory.SetLastAccessTime(path, lastAccessTime);

	public static void DirectorySetLastAccessTimeUtc(this string path, DateTime lastAccessTimeUtc) =>
		Directory.SetLastAccessTimeUtc(path, lastAccessTimeUtc);

	public static DateTime DirectoryGetLastAccessTime(this string path) => Directory.GetLastAccessTime(path);

	public static DateTime DirectoryGetLastAccessTimeUtc(this string path) => Directory.GetLastAccessTimeUtc(path);

	public static string[] DirectoryGetFiles(this string path) => Directory.GetFiles(path);

	public static string[] DirectoryGetFiles(this string path, string searchPattern) =>
		Directory.GetFiles(path, searchPattern);

	public static string[] DirectoryGetFiles(this string path, string searchPattern, SearchOption searchOption) =>
		Directory.GetFiles(path, searchPattern, searchOption);

	public static string[] DirectoryGetFiles(
		this string path,
		string searchPattern,
		EnumerationOptions enumerationOptions) => Directory.GetFiles(path, searchPattern, enumerationOptions);

	public static string[] DirectoryGetDirectories(this string path) => Directory.GetDirectories(path);

	public static string[] DirectoryGetDirectories(this string path, string searchPattern) =>
		Directory.GetDirectories(path, searchPattern);

	public static string[] DirectoryGetDirectories(
		this string path,
		string searchPattern,
		SearchOption searchOption) => Directory.GetDirectories(path, searchPattern, searchOption);

	public static string[] DirectoryGetDirectories(
		this string path,
		string searchPattern,
		EnumerationOptions enumerationOptions) => Directory.GetDirectories(path, searchPattern, enumerationOptions);

	public static string[] DirectoryGetFileSystemEntries(this string path) => Directory.GetFileSystemEntries(path);

	public static string[] DirectoryGetFileSystemEntries(this string path, string searchPattern) =>
		Directory.GetFileSystemEntries(path, searchPattern);

	public static string[] DirectoryGetFileSystemEntries(
		this string path,
		string searchPattern,
		SearchOption searchOption) => Directory.GetFileSystemEntries(path, searchPattern, searchOption);

	public static string[] DirectoryGetFileSystemEntries(
		this string path,
		string searchPattern,
		EnumerationOptions enumerationOptions) =>
		Directory.GetFileSystemEntries(path, searchPattern, enumerationOptions);

	public static IEnumerable<string> DirectoryEnumerateDirectories(this string path) =>
		Directory.EnumerateDirectories(path);

	public static IEnumerable<string> DirectoryEnumerateDirectories(this string path, string searchPattern) =>
		Directory.EnumerateDirectories(path, searchPattern);

	public static IEnumerable<string> DirectoryEnumerateDirectories(
		this string path,
		string searchPattern,
		SearchOption searchOption) => Directory.EnumerateDirectories(path, searchPattern, searchOption);

	public static IEnumerable<string> DirectoryEnumerateDirectories(
		this string path,
		string searchPattern,
		EnumerationOptions enumerationOptions) =>
		Directory.EnumerateDirectories(path, searchPattern, enumerationOptions);

	public static IEnumerable<string> DirectoryEnumerateFiles(this string path) => Directory.EnumerateFiles(path);

	public static IEnumerable<string> DirectoryEnumerateFiles(this string path, string searchPattern) =>
		Directory.EnumerateFiles(path, searchPattern);

	public static IEnumerable<string> DirectoryEnumerateFiles(
		this string path,
		string searchPattern,
		SearchOption searchOption) => Directory.EnumerateFiles(path, searchPattern, searchOption);

	public static IEnumerable<string> DirectoryEnumerateFiles(
		this string path,
		string searchPattern,
		EnumerationOptions enumerationOptions) => Directory.EnumerateFiles(path, searchPattern, enumerationOptions);

	public static IEnumerable<string> DirectoryEnumerateFileSystemEntries(this string path) =>
		Directory.EnumerateFileSystemEntries(path);

	public static IEnumerable<string> DirectoryEnumerateFileSystemEntries(this string path, string searchPattern) =>
		Directory.EnumerateFileSystemEntries(path, searchPattern);

	public static IEnumerable<string> DirectoryEnumerateFileSystemEntries(
		this string path,
		string searchPattern,
		SearchOption searchOption) => Directory.EnumerateFileSystemEntries(path, searchPattern, searchOption);

	public static IEnumerable<string> DirectoryEnumerateFileSystemEntries(
		this string path,
		string searchPattern,
		EnumerationOptions enumerationOptions) =>
		Directory.EnumerateFileSystemEntries(path, searchPattern, enumerationOptions);

	public static string DirectoryGetDirectoryRoot(this string path) => Directory.GetDirectoryRoot(path);

	public static void DirectorySetCurrentDirectory(this string path) => Directory.SetCurrentDirectory(path);

	public static void DirectoryMove(this string sourceDirName, string destDirName) =>
		Directory.Move(sourceDirName, destDirName);

	public static void DirectoryDelete(this string path) => Directory.Delete(path);

	public static void DirectoryDelete(this string path, bool recursive) => Directory.Delete(path, recursive);

	public static FileSystemInfo DirectoryCreateSymbolicLink(this string path, string pathToTarget) =>
		Directory.CreateSymbolicLink(path, pathToTarget);

}