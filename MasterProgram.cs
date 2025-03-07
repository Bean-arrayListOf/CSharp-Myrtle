using CSharp_Myrtle.Citrus;
using CSharp_Myrtle.Citrus.LibCPlatform;

using System.Data.SqlClient;
using System.Data.SQLite;
using System.Data.SqlTypes;
using System.Reflection;

namespace CSharp_Myrtle
{
    internal class MasterProgram
    {
        static void Main(string[] args)
        {
            using var mapdb = new MapDB("C:\\Users\\Hsdnm\\DataGripProjects\\identifier.sqlite");
            mapdb.Remove("1","1").OutLine();
        }
    }
}
