using System.Diagnostics;
using System.Text;

namespace CSharp_Myrtle
{
    internal class MasterProgram
    {
        static void Main(string[] args)
        {
            var info = new ProcessStartInfo
            {
                FileName = "/bin/zsh",
                Arguments = "--login",
                RedirectStandardOutput = false,
                UseShellExecute = true,
                CreateNoWindow = false
            };

            using var process = new Process();
            process.StartInfo = info;
            process.Start();

            //p(process.StandardOutput.BaseStream);

            process.WaitForExit();
        }

        public static async void p(Stream stream)
        {
            var buffer = new byte[8];
            var n = 0;
            while ((n = stream.Read(buffer)) > 0)
            {
                Console.Write(Encoding.UTF8.GetString(buffer, 0, n));
            }
        }
    }
}