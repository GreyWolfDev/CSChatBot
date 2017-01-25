using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CSChatBot.Helpers;
using DB;
using DB.Extensions;
using DB.Models;
using ModuleFramework;
using Telegram.Bot.Types.Enums;

namespace CSChatBot.Modules
{
    [ModuleFramework.Module(Author = "parabola949", Name = "Base", Version = "1.0")]
    class Base
    {
        [ChatCommand(Triggers = new[] {"version"}, HelpText = "Gets the current build version / time")]
        public static CommandResponse GetVersion(CommandEventArgs args)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fvi.FileVersion;
            DateTime dt =
                new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Local).AddDays(fvi.ProductBuildPart)
                    .AddSeconds(fvi.ProductPrivatePart*2)
                    .ToLocalTime();
            return new CommandResponse($"Current Version: {version}\nBuild time: {dt}");
        }

        [ChatCommand(Triggers = new[] {"source"}, HelpText = "Gets the source code for this bot")]
        public static CommandResponse GetSource(CommandEventArgs args)
        {
            return new CommandResponse("https://github.com/GreyWolfDev/CSChatBot");
        }

        [ChatCommand(Triggers = new[] {"modules"}, HelpText = "Show a list of modules currently loaded")]
        public static CommandResponse GetModules(CommandEventArgs args)
        {
            var sb = new StringBuilder();
            foreach (var m in Loader.Modules)
                sb.AppendLine($"{m.Key.Name}, by {m.Key.Author} (version {m.Key.Version})");
            return new CommandResponse(sb.ToString());
        }

        [ChatCommand(Triggers = new[] {"commands"}, HelpText = "commands <module name> - show all commands in the module")]
        public static CommandResponse GetCommands(CommandEventArgs args)
        {
            var sb = new StringBuilder();
            var module =
                Loader.Modules.FirstOrDefault(
                    x => String.Equals(x.Key.Name, args.Parameters, StringComparison.CurrentCultureIgnoreCase));
            if (module.Key == null)
                return new CommandResponse($"{args.Parameters} module not found.");

            foreach (var method in module.Value.GetMethods().Where(x => x.IsDefined(typeof(ChatCommand))))
            {
                var att = method.GetCustomAttributes<ChatCommand>().First();
                sb.AppendLine($"*{att.Triggers[0]}*: {att.HelpText??method.Name}");
            }

            return new CommandResponse(sb.ToString(), parseMode: ParseMode.Markdown);
        }



        public Base(Instance db, Setting settings)
        {

        }
    }
}
