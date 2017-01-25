using System;
using System.Threading;
using DB;
using DB.Models;
using ModuleFramework;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace CSChatBot.Modules
{
    [Module(Author = "parabola949", Name = "Test", Version = "1.0")]
    class TestModule
    {
        private static TelegramBotClient _bot;
        public TestModule(Instance db, Setting settings, TelegramBotClient bot)
        {
            _bot = bot;
            //since your constructor is passed an instance of the bot, you can also subscribe to updates:
            _bot.OnUpdate += delegate (object sender, UpdateEventArgs args)
            {
                new Thread(() => { HandleUpdate(args.Update); }).Start();
            };

            //you can do other things as well, like manipulate the database, add settings, etc.  Check the Weather Module for more samples.
        }

        [ChatCommand(Triggers = new[] { "test" }, BotAdminOnly = true)]
        public static CommandResponse DoTest(CommandEventArgs args)
        {
            //This will send a command along without returning
            //First we build our message
            var message = new MessageSentEventArgs
            {
                Response = new CommandResponse("This is a test command"),
                Target = args.Target //TODO Check this, not even sure it's in use in Telegram
            };
            //Now we send it
            args.Messenger.SendMessage(message);

            //we can also interact directly with the bot api as such
            args.Bot.SendTextMessageAsync(args.Message.Chat.Id, "This is a test button", replyMarkup: new InlineKeyboardMarkup(new[]
            {
                new InlineKeyboardButton("Para's Thoughts")
                {
                    Url="https://t.me/para949"
                },
                //can also use conditional statements when building menus
                true ? new InlineKeyboardButton("test") : null
            }));

            //now, return the method.  This can be null, in which case no text will be sent.
            return new CommandResponse("test complete?");


            //Why have all these ways to send messages?  The idea here is flexibility, ease of use, but also the ability to really drill down into the bot if needed.
        }


        public void HandleUpdate(Update u)
        {
            //handle an update here

            //I highly recommend you ignore old messages
            if (u.Type == UpdateType.MessageUpdate)
            {
                if (u.Message.Date < DateTime.UtcNow.AddSeconds(-15)) return;

                //_bot.SendTextMessageAsync(u.Message.Chat.Id, u.Message.Text);
            }
            
        }
    }
}
