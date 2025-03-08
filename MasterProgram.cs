using CSharp_Myrtle.Citrus;

namespace CSharp_Myrtle
{
    internal class MasterProgram
    {
        static void Main(string[] args)
        {
            Console.WriteLine(Env.cr.GetString("ProjectName"));
        }
    }
}