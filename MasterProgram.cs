using CSharp_Myrtle.Citrus;

namespace CSharp_Myrtle
{
    internal class MasterProgram
    {
        static void Main(string[] args)
        {
            var res = typeof(MasterProgram).GetResource("CSharp_Myrtle.MasterResource");
            res.GetString("ProjectName").OutLine();
        }
    }
}
