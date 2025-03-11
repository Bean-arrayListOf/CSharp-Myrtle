using System.Runtime.InteropServices;

namespace CSharp_Myrtle.Citrus.LibCPlatform.OS;

public class Windows
{
	[DllImport("msvcrt", EntryPoint = "system")]
	public static extern int System(string command);
}