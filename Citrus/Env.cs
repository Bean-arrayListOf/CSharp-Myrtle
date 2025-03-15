using System.Resources;

using CSharp_Myrtle.Properties;

namespace CSharp_Myrtle.Citrus;

public static class Env
{
	public static readonly ResourceManager cr = Master.ResourceManager;
	public static readonly string TempPath = Path.GetTempPath();
	public readonly static HashMode sha = (HashMode)cr.GetString("sha")!.ToInt();
	public readonly static int randomByteLength = cr.GetString("RandomByteLength")!.ToInt();

	public static string? Get(string key) => Environment.GetEnvironmentVariable(key);
}