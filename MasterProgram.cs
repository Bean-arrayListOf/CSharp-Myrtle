using CSharp_Myrtle.Citrus;

namespace CSharp_Myrtle
{
    internal class MasterProgram
    {
        static void Main(string[] args)
        {
            Kit.PrintTable(["1", "2", "3"], [["我", "m1.1", "m1.2"], ["m2", "我", "m2.2"], ["m3", "m3.1", "我"]])
                .OutLine();
            // 4
        }
    }
}