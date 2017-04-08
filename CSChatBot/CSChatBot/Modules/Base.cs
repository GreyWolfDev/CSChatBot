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
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace CSChatBot.Modules
{
    [ModuleFramework.Module(Author = "parabola949", Name = "Base", Version = "1.0")]
    class Base
    {
        [ChatCommand(Triggers = new[] { "start" }, HideFromInline = true, DontSearchInline = true)]
        public static CommandResponse Start(CommandEventArgs args)
        {
            return new CommandResponse($"Hi there, I'm {Telegram.Me.FirstName}!\nI'm an open source modular chat bot.  You can find my source code [here](https://github.com/GreyWolfDev/CSChatBot).\nUse /modules to see what modules I have available, and `/commands <modulename>` to see what commands are in a specific module.", parseMode: ParseMode.Markdown);
        }

        [ChatCommand(Triggers = new[] { "version" }, HelpText = "Gets the current build version / time")]
        public static CommandResponse GetVersion(CommandEventArgs args)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fvi.FileVersion;
            DateTime dt =
                new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Local).AddDays(fvi.ProductBuildPart)
                    .AddSeconds(fvi.ProductPrivatePart * 2)
                    .ToLocalTime();
            return new CommandResponse($"Current Version: {version}\nBuild time: {dt}");
        }

        [ChatCommand(Triggers = new[] { "source" }, HelpText = "Gets the source code for this bot")]
        public static CommandResponse GetSource(CommandEventArgs args)
        {
            return new CommandResponse("https://github.com/GreyWolfDev/CSChatBot");
        }

        [ChatCommand(Triggers = new[] { "modules" }, HelpText = "Show a list of modules currently loaded")]
        public static CommandResponse GetModules(CommandEventArgs args)
        {
            //var sb = new StringBuilder();
            var menu = new Menu
            {
                Columns = 2,
                Buttons = Loader.Modules.Select(x => new InlineButton(x.Key.Name, "i", x.Key.Name)).ToList()
            };
            //foreach (var m in Loader.Modules)
            //    sb.AppendLine($"{m.Key.Name}, by {m.Key.Author} (version {m.Key.Version})");
            return new CommandResponse("Currently loaded modules: ", menu: menu, level: ResponseLevel.Private);
        }

        [CallbackCommand(Trigger = "i", HelpText = "Gets information on a module")]
        public static CommandResponse GetCommands(CallbackEventArgs args)
        {
            var sb = new StringBuilder();
            var m =
                Loader.Modules.FirstOrDefault(
                    x => String.Equals(x.Key.Name, args.Parameters, StringComparison.CurrentCultureIgnoreCase));
            if (m.Key == null)
                return new CommandResponse($"{args.Parameters} module not found.");
            sb.AppendLine($"*{m.Key.Name}*, by _{m.Key.Author}_ (version {m.Key.Version})\n");
            var menu = new Menu { Columns = 2 };
            foreach (var method in m.Value.GetMethods().Where(x => x.IsDefined(typeof(ChatCommand))))
            {
                var att = method.GetCustomAttributes<ChatCommand>().First();
                menu.Buttons.Add(new InlineButton(att.Triggers[0], "c", att.Triggers[0]));
            }

            return new CommandResponse(sb.ToString(), parseMode: ParseMode.Markdown, menu: menu);
        }

        [CallbackCommand(Trigger = "c", HelpText = "Gets information on a command")]
        public static CommandResponse GetCommandInfo(CallbackEventArgs args)
        {
            var sb = new StringBuilder();
            var c =
                Loader.Commands.FirstOrDefault(
                    x => String.Equals(x.Key.Triggers[0], args.Parameters, StringComparison.CurrentCultureIgnoreCase)).Key;
            if (c == null)
                return new CommandResponse($"{args.Parameters} command not found.");
            sb.AppendLine($"*{c.Triggers[0]}*: {c.HelpText}");
            if (c.Parameters.Length > 0)
                sb.AppendLine("*Parameters*");
            foreach (var p in c.Parameters)
                sb.AppendLine($"\t{p}");
            return new CommandResponse(sb.ToString(), parseMode: ParseMode.Markdown);
        }

        [ChatCommand(Triggers = new[] { "commands" }, HelpText = "commands <module name> - show all commands in the module", Parameters = new[] { "<module name>" })]
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
                sb.AppendLine($"*{att.Triggers[0]}*: {att.HelpText ?? method.Name}");
            }

            return new CommandResponse(sb.ToString(), parseMode: ParseMode.Markdown);
        }



        public Base(Instance db, Setting settings, TelegramBotClient bot)
        {

        }
    }
}
