![](http://tgwerewolf.com/logo.png)
# CSChatBot

CSChatBot is a bot framework for .NET devs looking to build a Telegram Bot

  - Fully Modular - Build your modules quickly and simply
  - Dynamic Database - Modules can easily modify / change the database schema
  - Simple to use - even comes with a module template that you can install into Visual Studio

## Requirements

  - This solution is built in VS2017, though modules can be built in any version of VS
  - This is a native .NET application, not dotnet core, so must run on Windows

> My overall goal with CSChatBot was
> to provide a fast, simple solution
> for other developers who didn't
> completely understand how Telegram
> Bot API works.
>        - Para

### Setup
You can either download a release build, or clone the repo locally and compile yourself.  When you start the bot, it will guide you through some basic setup, like getting your bot token ([from BotFather](https://t.me/BotFather)) and asking for any needed API keys.

### Building your own module
There is a module template available in the releases.  You can import than into VS, or just use the ModuleTemplate project included in the source.

#### Basic setup
If you choose to create your own project, rather than using the template, you will need to install the Telegram.Bot library from Nuget
```
Install-Package Telegram.Bot -Pre
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


### Modules

There are a few modules included with CSChatBot

| Module | Description | Project |
| ------ | ------ | ------ |
| Admin | Basic admin commands | CSChatBot |
| Base | Basic commands, like !modules | CSChatBot |
| Basic | Simple commands, like !lmgtfy | CSChatBot |
| User Functions | Basic user commands, like !points | CSChatBot |
| Mitsuku | A chat bot users can chat with | Cleverbot |
| Misc | Random stuff, like NSFW image detection and other things | Misc |
| Weather | A sample module that gets the weather for the user | WeatherModule |
| XKCD | A module for searching XKCD of course :) | XKCD |


