using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CSharp_Myrtle.Subscription
{

    public delegate void Debug_Output(string message);
    public delegate void Debug_Exception(Exception exception);

    internal static class LibLoaderEventBuild
    {
        public static event Debug_Output OnDebugOutput;
        public static event Debug_Exception OnDebugException;

        private static string BaseDir = AppDomain.CurrentDomain.BaseDirectory;
        private static string BaseName = AppDomain.CurrentDomain.FriendlyName;

        public static List<string> DllLibPaths = new List<string>
        {
            Path.Combine(BaseDir, "libs"),
            Path.Combine(BaseDir, "plugins"),
            Path.Combine(BaseDir, "extensions"),
            Path.Combine(BaseDir, "modules"),
            Path.Combine(BaseDir, "addons"),
            Path.Combine(BaseDir, "bin"),
            Path.Combine(BaseDir, "dependencies"),
            Path.Combine(BaseDir, $"{BaseName}_Data"),
            Path.Combine(BaseDir, $"{BaseName}_Data","lib"),
            Path.Combine(BaseDir, $"{BaseName}_Data","libs"),
            Path.Combine(BaseDir, $"{BaseName}_Data","plugins"),
            Path.Combine(BaseDir, $"{BaseName}_Data","extensions"),
            Path.Combine(BaseDir, $"{BaseName}_Data","modules"),
            Path.Combine(BaseDir, $"{BaseName}_Data","addons"),
            Path.Combine(BaseDir, $"{BaseName}_Data","bin"),
            Path.Combine(BaseDir, $"{BaseName}_Data","dependencies")
        };

        private static bool _debug = false;

        private static void DebugOutput(string message)
        {
            if(OnDebugOutput == null)
            {
                OnDebugOutput += (msg) => { Console.WriteLine(msg); };
            }

            if (_debug)
            {
                OnDebugOutput?.Invoke(message);
            }
        }

        private static void DebugException(Exception exception)
        {
            if(OnDebugException != null)
            {
                OnDebugException += (ex) => { Console.WriteLine(ex.ToString()); };
            }

            if (_debug)
            {
                OnDebugException?.Invoke(exception);
            }
        }

        public static void DebugMode(bool debug)
        {
            _debug = debug;
        }

        public static ResolveEventHandler BuildEvent()
        {
            return CurrentDomain_AssemblyResolve;
        }

        private static Assembly? CurrentDomain_AssemblyResolve(object? sender, ResolveEventArgs args)
        {
            try
            {
                string? DllName = new AssemblyName(args.Name).Name;

                if (DllName == null)
                {
                    throw new ArgumentNullException(nameof(DllName));
                }

                string FileDllName = DllName + ".dll";

                DebugOutput($"Find: [n:{args.Name}, fn:{DllName}, f:{FileDllName}]");

                foreach (var dir in DllLibPaths)
                {
                    string path = Path.Combine(dir, FileDllName);
                    if (File.Exists(path))
                    {
                        DebugOutput($"Find-OK: [{path}]");
                        return Assembly.LoadFrom(path);
                    }
                    else
                    {
                        DebugOutput($"Find-NOT: [{path}]");
                        continue;
                    }
                }

                return null;
            }
            catch(Exception ex)
            {
                DebugException(ex);
                return null;
            }
        }
    }
}
