using CSharp_Myrtle.Citrus;

namespace CSharp_Myrtle
{
    internal class MasterProgram
    {
        static void Main(string[] args)
        {
            var start = Kit.TimeSeconds();


            var end = Kit.TimeSeconds();

            Console.WriteLine("{0}MS", (end - start));
        }
    }
}