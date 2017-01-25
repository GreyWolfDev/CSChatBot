using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB;
using DB.Models;
using ModuleFramework;
using Telegram.Bot;

namespace ModuleTemplate
{
    /// <summary>
    /// A sample module for CSChatBot
    /// </summary>
    [ModuleFramework.Module(Author = "your name", Name = "Name of Module", Version = "1.0")]
    public class ModuleName
    {
        public ModuleName(Instance db, Setting settings, TelegramBotClient bot)
        {
            //do your initialization here.  this is done before the bot begins receiving updates.
            //DO NOT run  bot.StartReceiving();
            //This is run in the main bot already.

            //You can subscribe to updates.  This is a simple echo
            //bot.OnUpdate += (sender, args) =>
            //{
            //    bot.SendTextMessageAsync(args.Update.Message.Chat.Id, args.Update.Message.Text);
            //};

            //Or other things, like callback queries (inline buttons)
            //bot.OnCallbackQuery += (sender, args) =>
            //{
            //    bot.SendTextMessageAsync(args.CallbackQuery.From.Id, args.CallbackQuery.Data);
            //};
        }

        [ChatCommand(Triggers = new[] { "test" }, DontSearchInline = true, HelpText = "This is a sample command")]
        public static CommandResponse Test(CommandEventArgs args)
        {
            //do something
            var length = args.Message.Text.Length;
            //return message
            return new CommandResponse($"Message: {args.Message.Text}\nLength: {length}");
        }
    }
}
