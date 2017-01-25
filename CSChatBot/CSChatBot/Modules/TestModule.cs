using DB;
using DB.Models;
using ModuleFramework;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace CSChatBot.Modules
{
    [Module(Author = "parabola949", Name = "Test", Version = "1.0")]
    class TestModule
    {
        public TestModule(Instance db, Setting settings)
        {

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
            args.Bot.SendTextMessageAsync(args.Message.Chat.Id, "This is a test button", replyMarkup: new InlineKeyboardMarkup(new []
            {
                new InlineKeyboardButton("Para's Thoughts")
                {
                    Url="https://t.me/para949"
                }, 
            }));

            //now, return the method.  This can be null, in which case no text will be sent.
            return new CommandResponse("test complete?");
        }
    }
}
