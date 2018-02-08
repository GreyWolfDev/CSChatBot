using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB;
using DB.Extensions;
using DB.Models;
using ModuleFramework;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

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
            bot.OnUpdate += (sender, args) =>
            {
                if (!(args.Update.Message?.Date > DateTime.UtcNow.AddSeconds(-15)))
                {
                    return;
                }
                if (args.Update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
                {
                    var m = args.Update.Message;
                    if (m.Chat.Type == Telegram.Bot.Types.Enums.ChatType.Private || m.Chat.Type == Telegram.Bot.Types.Enums.ChatType.Channel)
                        return;
                    if (m.Type == Telegram.Bot.Types.Enums.MessageType.Photo)
                    {
                        var g = GetGroup(db, args.Update);
                        if (g.GetSetting<bool>("AutoKickImage", db, false))
                        {
                            //kick user!
                            var mem = bot.GetChatMemberAsync(m.Chat.Id, m.From.Id).Result;
                            if (mem.Status == Telegram.Bot.Types.Enums.ChatMemberStatus.Member)
                            {
                                db.Log.WriteLine($"Kicking {m.From.FirstName} from {m.Chat.Title} for image");
                                bot.DeleteMessageAsync(m.Chat.Id, m.MessageId);
                                KickUser(m.Chat.Id, m.From.Id, bot);
                            }
                        }
                    }

                    if (m.Entities?.Any(x => x.Type == Telegram.Bot.Types.Enums.MessageEntityType.Url) ?? false)
                    {
                        //url sent
                        var g = GetGroup(db, args.Update);
                        if (g.GetSetting<bool>("AutoKickLink", db, false))
                        {
                            //kick user!
                            var mem = bot.GetChatMemberAsync(m.Chat.Id, m.From.Id).Result;
                            if (mem.Status == Telegram.Bot.Types.Enums.ChatMemberStatus.Member)
                            {
                                db.Log.WriteLine($"Kicking {m.From.FirstName} from {m.Chat.Title} for link");
                                bot.DeleteMessageAsync(m.Chat.Id, m.MessageId);
                                KickUser(m.Chat.Id, m.From.Id, bot);
                            }
                        }
                    }

                }
            };

            //Or other things, like callback queries (inline buttons)
            //bot.OnCallbackQuery += (sender, args) =>
            //{
            //    bot.SendTextMessageAsync(args.CallbackQuery.From.Id, args.CallbackQuery.Data);
            //};
        }

        [ChatCommand(Triggers = new[] { "cfggroup" }, DontSearchInline = true, HideFromInline = true, InGroupOnly = true, GroupAdminOnly = true, HelpText = "Configure Group Management features")]
        public static CommandResponse ConfigureGroup(CommandEventArgs args)
        {
            var g = args.DatabaseInstance.GetGroupById(args.Message.Chat.Id);

            var m = CreateMenu(g, args.DatabaseInstance);
            return new CommandResponse("Choose what to configure", ResponseLevel.Private, m);
        }

        [CallbackCommand(Trigger = "ts")]
        public static CommandResponse ToggleSetting(CallbackEventArgs args)
        {
            var groupid = int.Parse(args.Parameters.Split('|')[0]);
            var g = args.DatabaseInstance.GetGroup(groupid);
            var setting = args.Parameters.Split('|')[1];
            var set = g.GetSetting<bool>(setting, args.DatabaseInstance, false);
            set = !set;
            g.SetSetting<bool>(setting, args.DatabaseInstance, false, set);
            g = args.DatabaseInstance.GetGroup(groupid);
            Menu m;
            if (args.Query.Message.Text.Contains("Auto Kick"))
                m = CreateAutoKickMenu(g, args.DatabaseInstance);
            else
                m = CreateMenu(g, args.DatabaseInstance);

            args.Bot.EditMessageReplyMarkupAsync(args.SourceUser.UserId, args.Query.Message.MessageId, CreateMarkupFromMenu(m));
            //return new CommandResponse("Choose what to configure", ResponseLevel.Private, m);
            return null;
        }

        [CallbackCommand(Trigger = "done")]
        public static CommandResponse Done(CallbackEventArgs a)
        {
            a.Bot.DeleteMessageAsync(a.SourceUser.UserId, a.Query.Message.MessageId);
            return null;
        }

        [CallbackCommand(Trigger = "back")]
        public static CommandResponse Back(CallbackEventArgs a)
        {
            var g = a.DatabaseInstance.GetGroup(int.Parse(a.Parameters));
            var m = CreateMenu(g, a.DatabaseInstance);
            a.Bot.EditMessageTextAsync(a.SourceUser.UserId, a.Query.Message.MessageId, "Choose what to configure", replyMarkup: CreateMarkupFromMenu(m));
            return null;
        }

        [CallbackCommand(Trigger = "autokick")]
        public static CommandResponse AutoKickMenu(CallbackEventArgs a)
        {
            var g = a.DatabaseInstance.GetGroup(int.Parse(a.Parameters));
            var m = CreateAutoKickMenu(g, a.DatabaseInstance);
            a.Bot.EditMessageTextAsync(a.SourceUser.UserId, a.Query.Message.MessageId, "Auto Kick Settings", replyMarkup: CreateMarkupFromMenu(m));
            return null;
        }

        private static Menu CreateMenu(Group g, Instance db)
        {
            var menu = new Menu(2, new List<InlineButton>());
            menu.Buttons.Add(new InlineButton("Auto Kick Settings", "autokick", g.ID.ToString()));
            var nsfw = g.GetSetting<bool>("NSFW", db, false);
            menu.Buttons.Add(new InlineButton($"NSFW : {(nsfw ? "✅" : "🚫")}", "ts", $"{g.ID}|NSFW"));

            menu.Buttons.Add(new InlineButton("Done", "done"));
            return menu;
        }

        private static Menu CreateAutoKickMenu(Group g, Instance db)
        {
            var menu = new Menu(2, new List<InlineButton>());

            var kick = g.GetSetting<bool>("AutoKickLink", db, false);
            menu.Buttons.Add(new InlineButton($"Auto Kick Links : {(kick ? "✅" : "🚫")}", "ts", $"{g.ID}|AutoKickLink"));
            kick = g.GetSetting<bool>("AutoKickImage", db, false);
            menu.Buttons.Add(new InlineButton($"Auto Kick Images : {(kick ? "✅" : "🚫")}", "ts", $"{g.ID}|AutoKickImage"));
            //kick = g.GetSetting<bool>("AutoKickImage", db, false);
            //menu.Buttons.Add(new InlineButton($"Auto Kick Images : {(kick ? "✅" : "🚫")}", "ts", $"{g.ID}|AutoKickImage"));

            menu.Buttons.Add(new InlineButton("Back", "back", g.ID.ToString()));
            return menu;
        }

        public static InlineKeyboardMarkup CreateMarkupFromMenu(Menu menu)
        {
            if (menu == null) return null;
            var col = menu.Columns - 1;
            //this is gonna be fun...
            var final = new List<InlineKeyboardButton[]>();
            for (var i = 0; i < menu.Buttons.Count; i++)
            {
                var row = new List<InlineKeyboardButton>();
                do
                {
                    var cur = menu.Buttons[i];
                    row.Add(new InlineKeyboardButton
                    {
                        Text = cur.Text,
                        CallbackData = $"{cur.Trigger}|{cur.ExtraData}",
                        Url = cur.Url
                    });
                    i++;
                    if (i == menu.Buttons.Count) break;
                } while (i % (col + 1) != 0);
                i--;
                final.Add(row.ToArray());
                if (i == menu.Buttons.Count) break;
            }
            return new InlineKeyboardMarkup(final.ToArray());
        }


        public static async Task KickUser(long chat, int user, TelegramBotClient bot)
        {




            try
            {
                await bot.KickChatMemberAsync(chat, user, DateTime.Now.AddSeconds(30));
            }
            catch (Exception e)
            {
                bot.SendTextMessageAsync(chat, e.Message);
            }

        }

        public static DB.Models.Group GetGroup(Instance db, Update update = null)
        {
            var from = update?.Message?.Chat;
            if (from == null) return null;
            var u = db.GetGroupById(from.Id) ?? new DB.Models.Group
            {
                GroupId = from.Id
            };
            u.Name = from.Title;
            u.UserName = from.Username;
            u.Save(db);
            return u;
        }
    }
}
