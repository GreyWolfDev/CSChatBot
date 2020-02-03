using Dapper;
using DB;
using DB.Extensions;
using DB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giveaway.Extensions
{
    public static class Extensions
    {
        #region giveaway
        public static void Save(this Models.Giveaway g, Instance db)
        {
            if (g.ID == null || !ExistsInDb(g, db))
            {
                //need to insert
                db.ExecuteNonQuery(
                    "insert into giveaway (Title, Description, Active, Owner, TimeStamp) VALUES (@Title, @Description, @Active, @Owner, @TimeStamp)",
                    g);
                g.ID =
                    db.Connection.Query<int>(
                        $"SELECT ID FROM Giveaway WHERE Description = @Description AND Owner = @Owner and TimeStamp = @TimeStamp", g)
                        .First();
            }
            else
            {
                db.ExecuteNonQuery(
                    "UPDATE giveaway SET Title = @Title, Description = @Description, Active = @Active, Owner = @Owner, MessageId = @MessageId, TimeStamp = @TimeStamp WHERE ID = @ID",
                    g);
            }
        }

        public static bool HasActiveGiveaway(this User u, Instance db)
        {
            var rows = db.Connection.Query($"SELECT COUNT(1) as 'Count' FROM giveaway WHERE Owner = '{u.UserId}' AND Active = 1");
            return (int)rows.First().Count > 0;
        }

        public static bool ExistsInDb(this Models.Giveaway g, Instance db)
        {
            var rows = db.Connection.Query($"SELECT COUNT(1) as 'Count' FROM giveaway WHERE ID = '{g.ID}'");
            return (int)rows.First().Count > 0;
        }

        public static void RemoveFromDb(this Models.Giveaway g, Instance db)
        {
            db.ExecuteNonQuery("DELETE FROM giveaway WHERE ID = @ID", g);
        }

        public static bool HasUser(this Models.Giveaway g, Instance db, int userid)
        {
            var rows = db.Connection.Query($"SELECT COUNT(1) as 'Count' FROM giveawayuser WHERE UserId = '{userid}' AND GiveawayId = '{g.ID}'");
            return (int)rows.First().Count > 0;
        }

        public static void AddUser(this Models.Giveaway g, Instance db, int userid)
        {
            db.ExecuteNonQuery(
                    $"insert into giveawayuser (UserId, GiveawayId) VALUES ({userid}, {g.ID})");
        }

        public static Models.Giveaway GetGiveaway(this Instance db, string id)
        {
            
            return db.Connection.Query<Models.Giveaway>($"select * from giveaway WHERE ID = {id} ").FirstOrDefault();
        }

        public static Models.Giveaway GetGiveaway(this Instance db, User u, bool active = true)
        {
            var a = active ? 1 : 0;
            return db.Connection.Query<Models.Giveaway>($"select * from giveaway WHERE Owner = {u.UserId} AND Active = {a} order by ID desc").FirstOrDefault();
        }

        public static IEnumerable<User> GetUsers(this Models.Giveaway g, Instance db)
        {
            var userIds = db.Connection.Query<int>($"select UserId from giveawayuser where GiveawayId = {g.ID}").ToList();
            return db.Users.Where(x => userIds.Contains(x.UserId));
        }
        #endregion
    }
}
