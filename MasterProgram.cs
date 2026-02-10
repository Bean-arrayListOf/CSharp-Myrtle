using CSharp_Myrtle.Subscription;

namespace CSharp_Myrtle
{
    internal static class MasterProgram
    {
        static void Main(string[] args)
        {
            LibLoaderEventBuild.DebugMode(true);
            AppDomain.CurrentDomain.AssemblyResolve += LibLoaderEventBuild.BuildEvent();
        }
    }
}