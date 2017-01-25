using Logger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CSChatBot.Helpers;
using CSChatBot.Modules;
using ModuleFramework;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace CSChatBot
{
    
    class Telegram
    {
        private static Log Log = new Log(Program.RootDirectory);
        public static TelegramBotClient Bot;

        public static async Task<bool> Run()
        {
            Bot = new TelegramBotClient(Program.LoadedSetting.TelegramBotAPIKey);
            await Bot.SetWebhookAsync();
            User me = null;
            try
            {
                me = Bot.GetMeAsync().Result;
            }
            catch (Exception e)
            {
                Log.WriteLine("502 bad gateway, restarting in 10 seconds", LogLevel.Error, fileName: "telegram.log");
                Thread.Sleep(TimeSpan.FromSeconds(10));
                //API is down... 
                return false;
            }
            //Bot.MessageReceived += BotOnMessageReceived;
            Bot.OnUpdate += BotOnUpdateReceived;
            Bot.StartReceiving();
            Log.WriteLine("Connected to Telegram and listening: " + me.Username);
            return true;
        }

        private static void BotOnUpdateReceived(object sender, UpdateEventArgs updateEventArgs)
        {
            try
            {
                var update = updateEventArgs.Update;

                if (!(update.Message.Date.AddHours(-5) > DateTime.Now.AddMinutes(-1)))
                {
                    //Log.WriteLine("Ignoring message due to old age: " + update.Message.Date);
                    return;
                }
                new Thread(() => Handle(update)).Start();
            }
            catch (Exception e)
            {
                while (e.InnerException != null)
                    e = e.InnerException;
                var message = e.GetType() + " - " + e.Message;
                if (e is FileNotFoundException)
                    message += " file: " + ((FileNotFoundException)e).FileName;
                //else if (e is DirectoryNotFoundException)
                //    message += " file: " + ((DirectoryNotFoundException)e).;
                message += Environment.NewLine + e.StackTrace;
                Log.WriteLine($"Error in message handling: {message}", LogLevel.Error, fileName: "telegram.log");
            }
        }

        internal static void Handle(Update update)
        {
            if (update.Message.Type == MessageType.TextMessage)
            {
                //TODO: do something with this update
                var msg = update.Message.From.Username??update.Message.From.FirstName + ": " + update.Message.Text;
                var chat = update.Message.Chat.Title;
                if (String.IsNullOrWhiteSpace(chat))
                    chat = "Private Message";

                var user = UserHelper.GetTelegramUser(Program.DB, update);

                Log.WriteLine(chat, LogLevel.Info, ConsoleColor.Cyan, "telegram.log");
                Log.WriteLine(msg, LogLevel.Info, ConsoleColor.White, "telegram.log");


                if (update.Message.Text.StartsWith("!") || update.Message.Text.StartsWith("/"))
                {
                    var args = GetParameters(update.Message.Text);
                    foreach (var command in Loader.Commands)
                    {
                        if (command.Key.Contains(args[0]))
                        {
                            var eArgs = new CommandEventArgs
                            {
                                SourceUser = user,
                                DatabaseInstance = Program.DB,
                                Parameters = args[1],
                                Target = update.Message.Chat.Id.ToString(),
                                Messenger = Program.Messenger,
                                Bot = Bot
                            };
                            var response = command.Value.Invoke(eArgs);
                            if (!String.IsNullOrWhiteSpace(response.Text))
                                Send(response.Text, update);
                        }
                    }
                }
            }
        }

        private static string[] GetParameters(string input)
        {
            return input.Contains(" ") ? new string[] { input.Substring(1, input.IndexOf(" ")).Trim(), input.Substring(input.IndexOf(" ") + 1) } : new string[] { input.Substring(1).Trim(), null };
        }

        public static void Send(CommandResponse response, Update update)
        {
            
        }

        public static void Send(string text, Update update)
        {
            Program.Log.WriteLine("Replying: " + text, overrideColor: ConsoleColor.Yellow);
            //text = text.Replace("\n", "").Replace("\r", "");
            //Console.ForegroundColor = ConsoleColor.DarkYellow;
            //text = text.Replace(@"X5O!P%@AP[4\PZX54(P^)7CC)7}$EICAR-STANDARD-ANTIVIRUS-TEST-FILE!$H+H*",
            //    "I'm not gonna say that!!");
            try
            {
                //if (Quiet)
                //    return;
                //var color = "|7|";
                if (text.StartsWith("/me"))
                {
                    text = text.Replace("/me", "*") + "*";
                }
                if (text.StartsWith("/"))
                {
                    text = text.Substring(1);

                }
                Bot.SendTextMessageAsync(update.Message.Chat.Id, text);
                //Bot.SendTextMessage(update.Message.Chat.Id, text);
                return;
            }
            catch (Exception e)
            {
                //Logging.Write("Server error! restarting..");
                //Process.Start("csircbot.exe");
                //Environment.Exit(7);
            }
        }

        public static void Send(MessageSentEventArgs args)
        {
            Program.Log.WriteLine("Replying: " + args.Response.Text, overrideColor: ConsoleColor.Yellow);
            var text = args.Response.Text;
            try
            {
                if (text.StartsWith("/me"))
                {
                    text = text.Replace("/me", "*") + "*";
                }
                if (text.StartsWith("/"))
                {
                    text = text.Substring(1);

                }
                long targetId = 0;
                if (long.TryParse(args.Target, out targetId))
                    Bot.SendTextMessageAsync(targetId, text, replyMarkup: args.Response.Markup, parseMode: args.Response.ParseMode);
                //Bot.SendTextMessage(update.Message.Chat.Id, text);
                return;
            }
            catch (Exception e)
            {
                //Logging.Write("Server error! restarting..");
                //Process.Start("csircbot.exe");
                //Environment.Exit(7);
            }
        }
    }
}
