using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CSChatBot.Modules;
using DB;
using DB.Extensions;
using DB.Models;
using Logger;
using ModuleFramework;

namespace CSChatBot
{
    class Program
    {

        public static string RootDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        public static Log Log = new Log(Path.Combine(RootDirectory, "Logs"));
        public static Instance DB = new Instance(Path.Combine(RootDirectory, "BotDB.sqlite"), Log.Path);
        public static Setting LoadedSetting;
        public static ModuleMessenger Messenger = new ModuleMessenger();

        static void Main(string[] args)
        {
            System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
            Console.OutputEncoding = Encoding.UTF8;
            Messenger.MessageSent += MessengerOnMessageSent;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            if (args.Length > 0)
            {
                try
                {
                    var idx = 0;
                    foreach (var arg in args.Where(arg => arg.StartsWith("-")))
                    {
                        switch (arg)
                        {
                            case "-useconfig":
                            case "--useconfig":
                                idx++;
                                LoadSettings(args[idx]);
                                break;
                            default:
                                PrintHelp();
                                break;
                            case "-setconfig":
                            case "--setconfig":
                                var alias = "Primary";

                                if (args.Length > idx + 1 && !args[idx + 1].StartsWith("-"))
                                {
                                    idx++;
                                    alias = args[idx];
                                }
                                RunConfiguration(alias);
                                break;

                        }
                        idx++;
                    }
                }
                catch
                {
                    PrintHelp();
                    
                }
            }

            if (LoadedSetting == null)
            {
                LoadSettings("Primary");
            }

            

            //everything should be initialized and ready to go now, time to run our listeners / connect to services

            var retry = false;
            do
            {
                retry = Telegram.Run().Result;
            } while (retry);


            new Task(MemberCountWatch).Start();

            Thread.Sleep(-1);
        }

        private static void MemberCountWatch()
        {
            while (true)
            {
                try
                {
                    var groups = DB.Groups.Select(x => x.GroupId);
                    foreach (var g in groups)
                    {
                        var count = Telegram.Bot.GetChatMembersCountAsync(g).Result;
                        var grp = DB.GetGroupById(g);
                        grp.MemberCount = count;
                        grp.Save(DB);
                        Thread.Sleep(1000);
                    }
                }
                catch
                {
                    // ignored
                }

                Thread.Sleep(TimeSpan.FromHours(1));
            }
        }


        private static void MessengerOnMessageSent(object sender, EventArgs e)
        {
            var args = (e as MessageSentEventArgs);
            Telegram.Send(args);
        }

        private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Log.WriteLine((e.ExceptionObject as Exception).Message, LogLevel.Error);
        }

        private static void PrintHelp()
        {
            //throw new NotImplementedException("PrintHelp");
            Console.Clear();
            Console.WriteLine("CSChatBot.exe Usage Information");
            Console.WriteLine("\t-useconfig <config alias>\tLoad the bot with a specific\n\t\t\t\t\tconfiguration\n");
            Console.WriteLine("\t-setconfig <config alias>\tRuns the configuration wizard for the\n\t\t\t\t\talias (creating the alias if needed),\n\t\t\t\t\tthen proceeds to run with that alias.\n");
            Console.Read();
            Environment.Exit(0);
        }

        private static void LoadSettings(string alias)
        {
            LoadedSetting = DB.Settings.FirstOrDefault(x => x.Alias == alias);
            if (LoadedSetting != null) return;
            Log.WriteLine($"Settings with alias {alias} not found.", LogLevel.Warn);
            Console.ForegroundColor = ConsoleColor.Gray;
            var key = ConsoleKey.A;
            while (key != ConsoleKey.Y && key != ConsoleKey.N)
            {
                Console.Write("\nWould you like to set up this configuration alias now? (Y|N): ");
                key = Console.ReadKey().Key;
                switch (key)
                {
                    case ConsoleKey.Y:
                        RunConfiguration(alias);
                        break;
                    case ConsoleKey.N:
                        Console.WriteLine("-");
                        Log.WriteLine("\n\nOpted not to configure, exiting as no settings are loaded.");
                        Thread.Sleep(1000);
                        Environment.Exit(0);
                        break;
                }
            }
        }

        private static void RunConfiguration(string alias)
        {
            LoadedSetting = DB.Settings.FirstOrDefault(x => x.Alias == alias) ?? new Setting { Alias = alias };
            
            string line;
            Log.WriteLine($"\nBeginning set up for configuration {LoadedSetting.Alias}");

            #region Get Telegram Settings

            var cont = false;
            while (!cont)
            {
                Console.Clear();
                Console.WriteLine("Press enter to keep the current value, or enter a new value");
                Console.Write(
                    $"Enter your Telegram User ID (use @userinfobot if you need) [Current: {LoadedSetting.TelegramDefaultAdminUserId}]: ");
                line = Console.ReadLine();
                var id = 0;
                if (!String.IsNullOrWhiteSpace(line) && int.TryParse(line, out id))
                    LoadedSetting.TelegramDefaultAdminUserId = id;
                if (LoadedSetting.TelegramDefaultAdminUserId + id != 0) cont = true;
            }
            Console.Clear();
            Console.WriteLine("Press enter to keep the current value, or enter a new value");
            Console.Write($"Enter your Telegram bot API key [Current: {LoadedSetting.TelegramBotAPIKey}]: ");
            line = Console.ReadLine();
            if (!String.IsNullOrWhiteSpace(line))
                LoadedSetting.TelegramBotAPIKey = line;

            #endregion

            Console.Clear();
            Log.WriteLine("Configuration input finished. Saving to database..");
            LoadedSetting.Save(DB);
        }
    }
}
