using DB;
using DB.Models;
using ModuleFramework;

namespace CSChatBot.Modules
{
    [ModuleFramework.Module(Author = "parabola949", Name = "Test", Version = "1.0")]
    class TestModule
    {
        public TestModule(Instance db, Setting settings)
        {

        }

        [ChatCommand(Triggers = new[] { "test" }, BotAdminOnly = true)]
        public static CommandResponse DoTest(CommandEventArgs args)
        {
            var message = new MessageSentEventArgs
            {
                Response = new CommandResponse("test things"),
                Target = args.Target
            };
            args.Messenger.SendMessage(message);

            return new CommandResponse("test complete?");
        }
    }
}
