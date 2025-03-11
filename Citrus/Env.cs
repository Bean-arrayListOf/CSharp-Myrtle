using System.Reflection;
using System.Resources;

namespace CSharp_Myrtle.Citrus;

public static class Env
{
	public static readonly ResourceManager cr = Properties.Master.ResourceManager;
	public static readonly string TempPath = Path.GetTempPath();
	public static string? Get(string key)
	{
		return Environment.GetEnvironmentVariable(key);
	}
}