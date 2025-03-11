using CSharp_Myrtle.Citrus.LibCPlatform.OS;

namespace CSharp_Myrtle.Citrus.LibCPlatform;

public class C
{

	public static int System(string command)
	{
		return Kit.Platform switch
		{
			OSType.MacOS => MacOS.System(command),
			OSType.Linux => Linux.System(command),
			OSType.Windows => OS.Windows.System(command),
			OSType.FreeBSD => Unix.System(command),
			_ => throw new Exception($"不支持系统:{Kit.Platform.ToString()}")
		};
	}
}