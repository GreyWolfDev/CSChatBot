using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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

        public static Log Log = new Logger.Log(Path.Combine(RootDirectory, "Logs"));
        public static Instance DB = new Instance(Path.Combine(RootDirectory, "BotDB.sqlite"), Log.Path);
        public static Setting LoadedSetting;
        public static ModuleMessenger Messenger = new ModuleMessenger();

        static void Main(string[] args)
        {
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
                catch (Exception e)
                {
                    PrintHelp();
                }
            }

            if (LoadedSetting == null)
            {
                LoadSettings("Primary");
            }

            //Load in the modules
            Loader.LoadModules();

            //everything should be initialized and ready to go now, time to run our listeners / connect to services

            var retry = false;
            do
            {
                Telegram.Run().ContinueWith((result) => retry = !result.Result);
            } while (retry);



            Thread.Sleep(-1);
        }


        private static void MessengerOnMessageSent(object sender, EventArgs e)
        {
            var args = (e as MessageSentEventArgs);
            Telegram.Send(args.Target, args.Response.Text);
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
            Console.Write("Would you like to set up this configuration alias now? (Y|N): ");
            if (Console.ReadKey().Key == ConsoleKey.Y)
            {
                RunConfiguration(alias);
            }
            else
            {
                Log.WriteLine("\nOpted not to configure, exiting as no settings are loaded.");
                Environment.Exit(0);
            }
        }

        private static void RunConfiguration(string alias)
        {
            LoadedSetting = DB.Settings.FirstOrDefault(x => x.Alias == alias) ?? new Setting { Alias = alias };
            ConsoleKey key;
            string line;
            Log.WriteLine($"\nBeginning set up for configuration {LoadedSetting.Alias}");

            #region Get Telegram Settings
            Console.Clear();
            Console.WriteLine("Press enter to keep the current value, or enter a new value");
            Console.Write($"Enter your Telegram User ID (use @userinfobot if you need) [Current: {LoadedSetting.TelegramDefaultAdminUserId}]: ");
            line = Console.ReadLine();
            var id = 0;
            if (!String.IsNullOrWhiteSpace(line) && int.TryParse(line, out id))
                LoadedSetting.TelegramDefaultAdminUserId = id;

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
