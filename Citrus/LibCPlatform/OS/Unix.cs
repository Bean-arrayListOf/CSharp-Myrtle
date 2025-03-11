using System.Runtime.InteropServices;

namespace CSharp_Myrtle.Citrus.LibCPlatform.OS;

public class Unix
{
	[DllImport("libc", EntryPoint = "system")]
	public static extern int System(string command);
}