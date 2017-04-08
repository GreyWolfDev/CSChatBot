using DB;
using DB.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace CSChatBot.Helpers
{
    public static class GroupHelper
    {
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
