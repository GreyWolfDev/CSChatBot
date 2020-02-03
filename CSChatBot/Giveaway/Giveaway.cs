using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB;
using DB.Models;
using Giveaway.Extensions;
using ModuleFramework;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Giveaway
{
    /// <summary>
    /// A sample module for CSChatBot
    /// </summary>
    [ModuleFramework.Module(Author = "Para", Name = "Giveaway", Version = "1.0")]
    public class Giveaway
    {
        static Random R = new Random();
        public Giveaway(Instance db, Setting settings, TelegramBotClient bot)
        {
            R.Next(100);
            try
            {

                new SQLiteCommand(
                    @"create table if not exists giveaway (ID INTEGER PRIMARY KEY AUTOINCREMENT, 
                    Title TEXT NOT NULL, Description TEXT, Active INTEGER DEFAULT 1, Owner INTEGER, MessageId TEXT, TimeStamp TEXT)", db.Connection)
                    .ExecuteNonQuery();
                new SQLiteCommand(
                    @"create table if not exists giveawayuser (UserId INTEGER, GiveawayId INTEGER)", db.Connection)
                    .ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        [ChatCommand(Triggers = new[] { "gastats" }, DontSearchInline = true)]
        public static CommandResponse GiveawayStats(CommandEventArgs args)
        {
            var g = args.DatabaseInstance.GetGiveaway(args.SourceUser);
            if (g == null)
                return new CommandResponse("You don't have an active giveaway to end");
            var users = g.GetUsers(args.DatabaseInstance).ToArray();
            if (users.Any())
            {
                return new CommandResponse($"Giveaway currently has {users.Count()} users");
            }
            return new CommandResponse("No users entered");
        }

        [ChatCommand(Triggers = new[] { "startga" }, DontSearchInline = true, HelpText = "Starts a giveaway", Parameters = new[] { "Title;", "Description" })]
        public static CommandResponse StartGiveaway(CommandEventArgs args)
        {
            if (args.SourceUser.HasActiveGiveaway(args.DatabaseInstance))
            {
                return new CommandResponse($"You already have an active giveway, please end it first!");
            }
            var things = args.Parameters.Split(';');
            if (things.Length != 2)
            {
                return new CommandResponse($"usage: Title here;Description here");
            }

            var g = new Models.Giveaway
            {
                Active = 1,
                Description = things[1],
                Owner = args.SourceUser.UserId,
                Title = things[0],
                TimeStamp = DateTime.Now.ToString()
            };

            g.Save(args.DatabaseInstance);

            var menu = new Menu
            {
                Columns = 1,
                Buttons = new List<InlineButton> { new InlineButton("Join", "joingiveaway", g.ID.ToString()) }
            };

            return new CommandResponse($"{g.Title}\r\n{g.Description}", menu: menu);
        }

        [ChatCommand(Triggers = new[] { "endga" }, DontSearchInline = true, HideFromInline = true, HelpText = "Ends a giveaway")]
        public static CommandResponse EndGiveaway(CommandEventArgs args)
        {
            var g = args.DatabaseInstance.GetGiveaway(args.SourceUser);
            if (g == null)
                return new CommandResponse("You don't have an active giveaway to end");
            g.Active = 0;
            g.Save(args.DatabaseInstance);
            var users = g.GetUsers(args.DatabaseInstance).ToArray();
            if (users.Any())
            {
                var winner = users[R.Next(users.Length)];
                args.Bot.EditMessageTextAsync(g.MessageId, $"{g.Title}\r\n{g.Description}\r\n\r\nThis giveaway has ended! The winner is {winner.Name}");

                return new CommandResponse($"{winner.Name}: @{winner.UserName} ({winner.UserId})");
            }
            return new CommandResponse("No users entered");
        }

        [ChatCommand(Triggers = new[] { "rerollga" }, DontSearchInline = true, HelpText = "Starts a giveaway", Parameters = new[] { "Title;", "Description" })]
        public static CommandResponse ReRollGiveaway(CommandEventArgs args)
        {
            var g = args.DatabaseInstance.GetGiveaway(args.SourceUser, false);
            if (g == null)
                return new CommandResponse("You don't have a giveaway to end");

            var users = g.GetUsers(args.DatabaseInstance).ToArray();
            if (users.Any())
            {
                var winner = users[R.Next(users.Length)];
                args.Bot.EditMessageTextAsync(g.MessageId, $"{g.Title}\r\n{g.Description}\r\n\r\nThis giveaway has ended! The winner is {winner.Name}");

                return new CommandResponse($"{winner.Name}: @{winner.UserName} ({winner.UserId})");
            }
            return new CommandResponse("No users entered");
        }

        [CallbackCommand(Trigger = "joingiveaway")]
        public static CommandResponse Join(CallbackEventArgs args)
        {
            var g = args.DatabaseInstance.GetGiveaway(args.Parameters);
            if (g == null)
                return new CommandResponse(callbackCaption: "Unknown giveaway");
            if (g?.MessageId == null)
            {
                g.MessageId = args.Query.InlineMessageId;
                g.Save(args.DatabaseInstance);
            }
            if (g.Active == 0)
            {
                return new CommandResponse(callbackCaption: "This giveaway has ended :(");
            }
            if (g.HasUser(args.DatabaseInstance, args.SourceUser.UserId))
            {
                return new CommandResponse(callbackCaption: "You've already joined this giveaway");
            }
            var menu = new Menu
            {
                Columns = 1,
                Buttons = new List<InlineButton> { new InlineButton("Join", "joingiveaway", g.ID.ToString()) }
            };
            g.AddUser(args.DatabaseInstance, args.SourceUser.UserId);
            var users = g.GetUsers(args.DatabaseInstance).ToArray();
            args.Bot.EditMessageTextAsync(g.MessageId, $"{g.Title}\r\n{g.Description}\r\n\r\nUsers joined: {users.Length}", replyMarkup: CreateMarkupFromMenu(menu));
            return new CommandResponse(callbackCaption: $"You have successfully joined the giveaway :)");
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
    }
}
