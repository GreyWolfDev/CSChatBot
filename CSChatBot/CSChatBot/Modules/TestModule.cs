using System;
using System.Collections.Generic;
using System.Threading;
using DB;
using DB.Models;
using ModuleFramework;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using DB.Extensions;
using System.Threading.Tasks;

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

        [ChatCommand(Triggers = new[] { "dotest" }, BotAdminOnly = true)]
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
                new InlineKeyboardButton()
                {
                    Text = "Grey Wolf Dev Channel",
                    Url="https://t.me/werewolfdev"
                },
                //can also use conditional statements when building menus
                true ? new InlineKeyboardButton() {Text = "Para's Channel", Url="https://t.me/para949"} : null
            }));

            // of course, if you saved the bot in your constructor, you can also use that
            Console.WriteLine(_bot.IsReceiving);

            //Another way to create a button menu is like this:
            var menu = new Menu
            {
                Columns = 3,
                Buttons = new List<InlineButton>
                {
                    new InlineButton("Column 1, Row 1", "test", "extra data 1"), //These buttons will trigger the callback command below
                    new InlineButton("Column 2, Row 1", "test", "extra data 2"),
                    new InlineButton("Column 3, Row 1", "test", "extra data 3"),
                    //This button will be the "remainder" (4 % 3 == 1) so it will span the bottom
                    new InlineButton("Grey Wolf Support", url: "https://t.me/werewolfsupport") //this button won't trigger a command, just opens the url.
                }
            };

            //now, return the method.  This can be null, in which case no text will be sent.
            // as you can see, we attached our custom menu
            return new CommandResponse("Here's my response!", ResponseLevel.Public, menu);


            //Why have all these ways to send messages?  The idea here is flexibility, ease of use, but also the ability to really drill down into the bot if needed.
        }

        [CallbackCommand(Trigger = "test", DevOnly = true)]
        public static CommandResponse CallbackTest(CallbackEventArgs args)
        {
            return new CommandResponse($"Group id: {args.Query.Message.Chat.Id}\nButton extra data: {args.Parameters}");
        }


        public void HandleUpdate(Update u)
        {
            //handle an update here

            //I highly recommend you ignore old messages
            if (u.Type == UpdateType.Message)
            {
                if (u.Message.Date < DateTime.UtcNow.AddSeconds(-15)) return;

                //_bot.SendTextMessageAsync(u.Message.Chat.Id, u.Message.Text);
            }

        }

        [ChatCommand(Triggers = new[] { "testgrpset" }, BotAdminOnly = true)]
        public static CommandResponse TestGroupSetting(CommandEventArgs args)
        {
            var m = args.Message;
            var id = m.Chat.Id;
            var group = args.DatabaseInstance.GetGroupById(id);
            var b = args.Bot;

            b.SendTextMessageAsync(id, "Getting boolean value, default false.");
            var value = group.GetSetting<bool>("TestBool", args.DatabaseInstance, false);
            Thread.Sleep(500);
            b.SendTextMessageAsync(id, $"Result: {value}\nSetting to true...");
            var success = group.SetSetting<bool>("TestBool", args.DatabaseInstance, false, true);
            value = group.GetSetting<bool>("TestBool", args.DatabaseInstance, false);
            Thread.Sleep(500);
            b.SendTextMessageAsync(id, $"Update result: {success}\nCurrent value for test setting: {value}\nSetting to false");

            success = group.SetSetting<bool>("TestBool", args.DatabaseInstance, false, false);
            value = group.GetSetting<bool>("TestBool", args.DatabaseInstance, false);
            Thread.Sleep(500);
            b.SendTextMessageAsync(id, $"Update result: {success}\nCurrent value for test setting: {value}");
            return null;

        }
    }
}
