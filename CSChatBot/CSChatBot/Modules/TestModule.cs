using DB;
using DB.Models;
using ModuleFramework;

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
                Target = args.Target
            };
            //Now we send it
            args.Messenger.SendMessage(message);

            //now, return the method.  This can be null, in which case no text will be sent.
            return new CommandResponse("test complete?");
        }
    }
}
