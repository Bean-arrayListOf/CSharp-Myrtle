using System.Drawing.Printing;
using System.Runtime.InteropServices;

using CSharp_Myrtle.Citrus;

namespace CSharp_Myrtle
{
	internal class MasterProgram
	{
		static void Main(string[] args)
		{
			Console.WriteLine(HashMode.SHA512.RandomHashHex());
		}

	}
}