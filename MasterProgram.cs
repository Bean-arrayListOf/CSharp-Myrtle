using System.Diagnostics;
using System.Text;

using CSharp_Myrtle.Citrus;

namespace CSharp_Myrtle
{
    internal class MasterProgram
    {
        static void Main(string[] args)
        {
            using var cab = new CacheAllocatorBuilder();

            cab.CreateFile("1.1");

            cab.GetRoot().OutLine();

            cab.GetKeys().OutLine();
;        }
    }
}