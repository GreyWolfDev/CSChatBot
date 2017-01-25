using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB;
using DB.Models;
using DB.Extensions;
using ModuleFramework;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace CSChatBot.Helpers
{
    class UserHelper
    {
        public static DB.Models.User GetTelegramUser(Instance db, Update update = null, InlineQuery query = null, bool logPoint = true)
        {
            var from = update?.Message.From ?? query?.From;
            if (from == null) return null;
            var u = db.Users.FirstOrDefault(x => x.UserId == from.Id) ?? new DB.Models.User
            {
                FirstSeen = DateTime.Now,
                Points = 0,
                Debt = 0,
                IsBotAdmin = Program.LoadedSetting.TelegramDefaultAdminUserId == from.Id,
                UserId = from.Id
            };
            u.UserName = from.Username;
            if (query?.Location != null)
                u.Location = $"{query.Location.Latitude},{query.Location.Longitude}";
            u.Name = (from.FirstName + " " + from.LastName).Trim();
            if (logPoint)
            {
                var where = update != null ? update.Message.Chat.Title ?? "Private" : "Using inline query";
                u.LastHeard = DateTime.Now;
                u.LastState = "talking in " + where;
                u.Points += update?.Message.Text.Length??0 * 10;
            }
            u.Save(db);
            return u;
        }



        //public static CommandResponse LinkUser(Instance db, DB.Models.User usr, Update update)
        //{
        //    //get the linking key
        //    try
        //    {
        //        var key = update.Message.Text.Split(' ')[1];
        //        var u = db.Users.FirstOrDefault(x => x.LinkingKey == key);
        //        u.TelegramUserID = update.Message.From.Username;
        //        u.LinkingKey = null;
        //        u.Save(db);
        //        MergeUsers(db, u, usr);
        //        return new CommandResponse("Account linked.  Welcome " + u.Nick);
        //    }
        //    catch
        //    {
        //        return new CommandResponse("Unable to verify your account.");
        //    }
        //}

        public static DB.Models.User GetTarget(CommandEventArgs args)
        {
            return args.Message.GetTarget(args.Parameters, args.SourceUser, args.DatabaseInstance);
        }

        //public static DB.Models.User MergeUsers(Instance db, DB.Models.User ircUser, DB.Models.User telegramUser)
        //{
        //    ircUser.TelegramUserID = telegramUser.TelegramUserID;
        //    ircUser.LinkingKey = null;
        //    ircUser.Points += telegramUser.Points;
        //    ircUser.Debt += telegramUser.Debt;
        //    ircUser.LastHeard = telegramUser.LastHeard;
        //    ircUser.LastState = telegramUser.LastState;
        //    telegramUser.RemoveFromDb(db);
        //    ircUser.Save(db);
        //    return ircUser;
        //}
    }
}
