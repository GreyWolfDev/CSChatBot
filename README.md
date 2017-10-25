![](http://tgwerewolf.com/logo.png)
# CSChatBot

CSChatBot is a bot framework for .NET devs looking to build a Telegram Bot

  - Fully Modular - Build your modules quickly and simply
  - Dynamic Database - Modules can easily modify / change the database schema
  - Simple to use - even comes with a module template that you can install into Visual Studio

## Requirements

  - This solution is built in VS2017, though modules can be built in any version of VS
  - This is a native .NET application, not dotnet core, so must run on Windows - I plan to move to core soon

> My overall goal with CSChatBot was
> to provide a fast, simple solution
> for other developers who didn't
> completely understand how Telegram
> Bot API works.
>        - Para

### Setup
You can either download a release build, or clone the repo locally and compile yourself.  When you start the bot, it will guide you through some basic setup, like getting your bot token ([from BotFather](https://t.me/BotFather)) and asking for any needed API keys.

Any modules you would like to use will need to be placed in the Addon Modules directory:
`root/Addon Modules/XKCD.dll` for example.

### Building your own module
There is a module template available in the releases.  You can import than into VS, or just use the ModuleTemplate project included in the source.

#### Basic setup
If you choose to create your own project, rather than using the template, you will need to install the Telegram.Bot.Core library from Nuget
```
Install-Package Telegram.Bot.Core -Pre
```
In addition to this, you will also need to reference some of the libraries from the source:
 - DB
 - Logger
 - ModuleFramework

You primary module class must have the `Module` attribute, as well as a few basic information variables:
```cs
[Module(Author = "My Name", Name = "MyModule", Version = "1.0")]
```

The constructor for this class must also accept the arguments as shown below:
```cs
public MyModule(Instance db, Setting settings, TelegramBotClient bot)
```

There are a couple attributes available to register your commands with the bot - `CallbackCommand` and `ChatCommand`.  You can check the TestModule in CSChatBot project for examples.  The `Triggers` variable will bot what the bot watches for - you can have multiple triggers for one command.

Again, Please look at the sample modules included.

#### Group Settings
This is a new feature, and allows for dynamically adding / setting group configuration.  You'll need to use the DB extensions to get the group, then you can get / set the setting you want.  It also allows for `int`, `bool`, and `string` types.  The last argument is the default value.  If the field does not exist in the `chatgroup` table, it will be created with the default value.
```cs
var g = myDbInstance.GetGroupById(<groupid>)
var someBool = g.GetSetting<bool>("SomeBool", myDbInstance, false);
var someInt = g.GetSetting<int>("SomeInt", myDbInstance, 0);
var someString = g.GetSetting<string>("SomeString", myDbInstance, "");
```

Setting a group setting is very similar:
```cs
var someBool = true;
var someInt = 5;
g.SetSetting<bool>("SomeBool", myDbInstance, false, someBool);
var success = g.SetSetting<int>("SomeInt", myDbInstance, 0, someInt);
```

### Chat Commands, Responses, and Inline Mode
All methods in your module should be constructed as such:
```cs
[ChatCommand(Triggers = new[] { "test" }, DontSearchInline = true, HelpText = "This is a sample command")]
public static CommandResponse Test(CommandEventArgs args)
{
    //do something
    var length = args.Message.Text.Length;
    //return message
    return new CommandResponse($"Message: {args.Message.Text}\nLength: {length}");
}
```

Triggers are the text a user can call the command with, both inline and in chat, using /command or !command.  HelpText will show up when the user is looking through available modules, or when using inline searching.

There are a few other variables in the Attributes you can use:

| Variable | Description |
| -------- | ----------- |
| `BotAdminOnly` | Only someone you have set as admin can use this command |
| `GroupAdminOnly` | Only an admin of the group the command is run in can use this |
| `DevOnly` | Only YOU can run it |
| `InGroupOnly` | Blocks from being used in PM |
| `InPrivateOnly` | Can only be run in PM, no groups |
| `HideFromInline` | Cannot be used at all in inline mode |
| `DontSearchInline` | Can be used in inline mode ONLY when trigger is typed exactly |

> Keep in mind, if you don't specify one of the Inline mode hiding methods, the command WILL be run anytime a user types the bot name.  If you have a command that might be API limited (like weather stuff), it would be a good idea to use DontSearchInline

As of commit #397d1529, CommandResponse can now also return a thumbnail and some more information.  If the user doesn't enter the command directly (@botname ), they will get the command and HelpText in the results:
![](https://i.imgur.com/Q4xEONw.png)

If they type the command in inline mode (@botname steam), they will get more information in the result:
![](https://i.imgur.com/GZ8LrCC.png)

### Callback Commands and Menus

You can send inline button menus easily with the `Menu` object. A quick example:
```cs
var menu = new Menu
{
    Columns = 2,
    Buttons = new[] {new InlineButton("My Text", "trigger", "some data"), new InlineButton("Grey Wolf Website", url: "http://github.com/GreyWolfDev") }
};
```

This will create two side by side buttons.  The first button will call the CallbackCommand with the trigger "`trigger`" and pass it "some data", while the second one will open github.  Let's take a look at the `CallbackCommand`

```cs
[CallbackCommand(Trigger = "trigger", HelpText = "Some Command")]
public static CommandResponse SampleCommand(CallbackEventArgs args)
{
    return new CommandResponse($"You pressed a button!  The data behind it was: {args.Parameters}");
}
```

In the menu above, when the user clicks the first button, the menu will go away and they will be sent the text "You pressed a button! The data behind it was: some data"


### Modules

There are a few modules included with CSChatBot

| Module | Description | Project |
| ------ | ------ | ------ |
| Admin | Basic admin commands | CSChatBot |
| Base | Basic commands, like !modules | CSChatBot |
| Basic | Simple commands, like !lmgtfy | CSChatBot |
| User Functions | Basic user commands, like !points | CSChatBot |
| Mitsuku | A chat bot users can chat with (INACTIVE / BROKEN) | Cleverbot |
| Misc | Random stuff, like NSFW image detection and other things | Misc |
| Weather | A sample module that gets the weather for the user | WeatherModule |
| XKCD | A module for searching XKCD of course :) | XKCD |
| Steam | Can pull profile information for yourself or others | Steam |

