using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DB;
using DB.Models;
using ModuleFramework;
using Module = ModuleFramework.Module;

namespace CSChatBot.Modules
{
    internal static class Loader
    {
        internal delegate CommandResponse ChatCommandMethod(CommandEventArgs args);

        internal static Dictionary<string[], ChatCommandMethod> Commands = new Dictionary<string[], ChatCommandMethod>();

        internal static Dictionary<Module, Type> Modules = new Dictionary<Module, Type>();
        internal static void LoadModules()
        {
            //load base methods first
            GetMethodsFromAssembly(Assembly.GetExecutingAssembly());

            Program.Log.WriteLine("Scanning Addon Modules directory for custom modules...", overrideColor: ConsoleColor.Cyan);
            var moduleDir = Path.Combine(Program.RootDirectory, "Addon Modules");

            Directory.CreateDirectory(moduleDir);
            //now load modules from directory
            foreach (var file in Directory.GetFiles(moduleDir, "*.dll"))
            {
                GetMethodsFromAssembly(Assembly.LoadFrom(file));
            }
        }

        private static void GetMethodsFromAssembly(Assembly assembly)
        {
            foreach (var type in assembly.GetTypes().Where(x => x.IsDefined(typeof(Module))))
            {
                var tAtt = type.GetCustomAttributes<Module>().First();
                Modules.Add(tAtt, type);
                Program.Log.WriteLine($"Loading commands from {type.Name} Module\nAuthor: {tAtt.Author}, Name: {tAtt.Name}, Version: {tAtt.Version}", overrideColor: ConsoleColor.DarkCyan);
                foreach (var method in type.GetMethods().Where(x => x.IsDefined(typeof(ChatCommand))))
                {
                    var att = method.GetCustomAttributes<ChatCommand>().First();
                    Commands.Add(att.Triggers, (ChatCommandMethod)Delegate.CreateDelegate(typeof(ChatCommandMethod), method));
                    Program.Log.WriteLine($"Loaded ChatCommand {method.Name}\n\t Trigger(s): {att.Triggers.Aggregate((a, b) => a + ", " + b)}", overrideColor: ConsoleColor.Green);
                }
                var constructor = type.GetConstructor(new[] { typeof(Instance), typeof(Setting) });
                constructor.Invoke(new object[] { Program.DB, Program.LoadedSetting });
            }
        }
    }
}
