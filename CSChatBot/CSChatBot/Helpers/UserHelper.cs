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

namespace CSChatBot.Helpers
{
    class UserHelper
    {
        public static DB.Models.User GetTelegramUser(Instance db, Update update, bool logPoint = true)
        {
            var u = db.Users.FirstOrDefault(x => x.UserId == update.Message.From.Id) ?? new DB.Models.User
            {
                FirstSeen = DateTime.Now,
                Points = 0,
                Debt = 0,
                IsBotAdmin = Program.LoadedSetting.TelegramDefaultAdminUserId == update.Message.From.Id
            };
            u.UserName = update.Message.From.Username;
            u.Name = (update.Message.From.FirstName + " " + update.Message.From.LastName).Trim();
            if (logPoint)
            {
                
                u.LastHeard = DateTime.Now;
                u.LastState = "talking in " + (update.Message.Chat.Title??"Private");
                u.Points += update.Message.Text.Length * 10;
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
            if (String.IsNullOrWhiteSpace(args.Parameters))
                return args.SourceUser;
            return args.DatabaseInstance.Users.FirstOrDefault(
                        x =>
                            String.Equals(x.UserName, args.Parameters.Replace("@", ""), StringComparison.InvariantCultureIgnoreCase) ||
                            String.Equals(x.UserId.ToString(), args.Parameters, StringComparison.InvariantCultureIgnoreCase));
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
