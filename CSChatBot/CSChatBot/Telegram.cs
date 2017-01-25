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
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InputMessageContents;

namespace CSChatBot
{

    class Telegram
    {
        private static Log Log = new Log(Program.RootDirectory);
        public static TelegramBotClient Bot;
        internal static User Me = null;
        public static async Task<bool> Run()
        {
            Bot = new TelegramBotClient(Program.LoadedSetting.TelegramBotAPIKey);
            await Bot.SetWebhookAsync();

            try
            {
                Me = Bot.GetMeAsync().Result;
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
            Bot.OnInlineQuery += BotOnOnInlineQuery;
            Bot.StartReceiving();
            Log.WriteLine("Connected to Telegram and listening: " + Me.Username);
            return true;
        }

        private static void BotOnOnInlineQuery(object sender, InlineQueryEventArgs inlineQueryEventArgs)
        {
            try
            {
                var query = inlineQueryEventArgs.InlineQuery;

                new Thread(() => HandleQuery(query)).Start();
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

        private static void HandleQuery(InlineQuery query)
        {
            var user = UserHelper.GetTelegramUser(Program.DB, null, query);
            Log.WriteLine("INLINE QUERY", LogLevel.Info, ConsoleColor.Cyan, "telegram.log");
            Log.WriteLine(user.Name + ": " + query.Query, LogLevel.Info, ConsoleColor.White, "telegram.log");
            var com = GetParameters("/" + query.Query);
            var choices =
                Loader.Commands.Where(x => x.Key.DevOnly != true && x.Key.BotAdminOnly != true && x.Key.GroupAdminOnly != true & !x.Key.HideFromInline & !x.Key.DontSearchInline && 
                x.Key.Triggers.Any(t => t.ToLower().Contains(com[0].ToLower())) &! x.Key.DontSearchInline).ToList();
            choices.AddRange(Loader.Commands.Where(x => x.Key.DontSearchInline && x.Key.Triggers.Any(t => String.Equals(t, com[0], StringComparison.InvariantCultureIgnoreCase))));
            var results = new List<InlineQueryResultArticle>();
            foreach (var c in choices)
            {
                var response = c.Value.Invoke(new CommandEventArgs
                {
                    SourceUser = user,
                    DatabaseInstance = Program.DB,
                    Parameters = com[1],
                    Target = "",
                    Messenger = Program.Messenger,
                    Bot = Bot,
                    Message = null
                });
                results.Add(new InlineQueryResultArticle()
                {
                    Description = c.Key.HelpText,
                    Id = Loader.Commands.ToList().IndexOf(c).ToString(),
                    Title = c.Value.Method.Name,
                    InputMessageContent = new InputTextMessageContent
                    {
                        DisableWebPagePreview = true,
                        MessageText = response.Text,
                        ParseMode = response.ParseMode
                    }
                });
            }
            var menu = results.Cast<InlineQueryResult>().ToArray();
            var result = Bot.AnswerInlineQueryAsync(query.Id, menu, 0, true).Result;

        }

        private static void BotOnUpdateReceived(object sender, UpdateEventArgs updateEventArgs)
        {
            try
            {
                var update = updateEventArgs.Update;
                if (update.Type == UpdateType.InlineQueryUpdate) return;
                if (!(update.Message.Date > DateTime.UtcNow.AddSeconds(-15)))
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
                var msg = (update.Message.From.Username ?? update.Message.From.FirstName) + ": " + update.Message.Text;
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
                        if (command.Key.Triggers.Contains(args[0]))
                        {
                            //check for access
                            var att = command.Key;
                            if (att.DevOnly && update.Message.From.Id != Program.LoadedSetting.TelegramDefaultAdminUserId)
                            {

                                Send(new CommandResponse("You are not the developer!"), update);
                                return;

                            }
                            if (att.BotAdminOnly & !user.IsBotAdmin)
                            {
                                Send(new CommandResponse("You are not a bot admin!"), update);
                                return;
                            }
                            if (att.GroupAdminOnly)
                            {
                                if (update.Message.Chat.Type == ChatType.Private)
                                {
                                    Send(new CommandResponse("You need to run this in a group"), update);
                                    return;
                                }
                                //is the user an admin of the group?
                                var status = Bot.GetChatMemberAsync(update.Message.Chat.Id, update.Message.From.Id).Result.Status;
                                if (status != ChatMemberStatus.Administrator && status != ChatMemberStatus.Creator)
                                {
                                    Send(new CommandResponse("You are not a group admin!"), update);
                                    return;
                                }
                            }
                            if (att.InGroupOnly && update.Message.Chat.Type == ChatType.Private)
                            {
                                Send(new CommandResponse("You need to run this in a group"), update);
                                return;
                            }
                            if (att.InPrivateOnly)
                            {
                                Send(new CommandResponse("You need to run this in private"), update);
                                return;
                            }
                            var eArgs = new CommandEventArgs
                            {
                                SourceUser = user,
                                DatabaseInstance = Program.DB,
                                Parameters = args[1],
                                Target = update.Message.Chat.Id.ToString(),
                                Messenger = Program.Messenger,
                                Bot = Bot,
                                Message = update.Message
                            };
                            var response = command.Value.Invoke(eArgs);
                            if (!String.IsNullOrWhiteSpace(response.Text))
                                Send(response, update);
                        }
                    }
                }
            }
        }

        private static string[] GetParameters(string input)
        {
            if (input.Length == 0) return new[] { "", "" };
            var result = input.Contains(" ") ? new string[] { input.Substring(1, input.IndexOf(" ")).Trim(), input.Substring(input.IndexOf(" ") + 1) } : new string[] { input.Substring(1).Trim(), null };
            result[0] = result[0].Replace("@" + Me.Username, "");
            return result;
        }

        public static void Send(CommandResponse response, Update update)
        {
            var text = response.Text;
            Program.Log.WriteLine("Replying: " + text, overrideColor: ConsoleColor.Yellow);
            try
            {
                if (text.StartsWith("/me"))
                {
                    text = text.Replace("/me", "*") + "*";
                }
                long targetId = response.Level == ResponseLevel.Public ? update.Message.Chat.Id : update.Message.From.Id;
                Bot.SendTextMessageAsync(targetId, text, replyMarkup: response.Markup, parseMode: response.ParseMode);
                //Bot.SendTextMessage(update.Message.Chat.Id, text);
                return;
            }
            catch (Exception e)
            {

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
