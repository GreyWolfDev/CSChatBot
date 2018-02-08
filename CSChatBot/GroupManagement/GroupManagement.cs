using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB;
using DB.Models;
using ModuleFramework;
using Telegram.Bot;

namespace GroupManagement
{
    /// <summary>
    /// A sample module for CSChatBot
    /// </summary>
    [ModuleFramework.Module(Author = "Para", Name = "Group Management", Version = "1.0")]
    public class GroupManagement
    {
        public GroupManagement(Instance db, Setting settings, TelegramBotClient bot)
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

        [ChatCommand(Triggers = new[] { "cfggroup" }, DontSearchInline = true, HideFromInline = true, InGroupOnly = true, GroupAdminOnly = true, HelpText = "This is a sample command")]
        public static CommandResponse ConfigureGroup(CommandEventArgs args)
        {
            return new CommandResponse($"Not Implemented");
        }
    }
}
