using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DB.Models;

namespace DB.Extensions
{
    public static class Extensions
    {
        #region Users

        public static void Save(this User u, Instance db)
        {
            if (u.ID == null || !ExistsInDb(u, db))
            {
                //need to insert
                db.ExecuteNonQuery(
                    "insert into users (Name, UserId, UserName, FirstSeen, LastHeard, Points, Location, Debt, LastState, Greeting, Grounded, GroundedBy, IsBotAdmin, LinkingKey, Description) VALUES (@Name, @UserId, @UserName, @FirstSeen, @LastHeard, @Points, @Location, @Debt, @LastState, @Greeting, @Grounded, @GroundedBy, @IsBotAdmin, @LinkingKey, @Description)",
                    u);
                u.ID =
                    db.Connection.Query<int>(
                        $"SELECT ID FROM Users WHERE UserId = @UserId", u)
                        .First();
            }
            else
            {
                db.ExecuteNonQuery(
                    "UPDATE users SET Name = @Name, UserId = @UserId, UserName = @UserName, FirstSeen = @FirstSeen, LastHeard = @LastHeard, Points = @Points, Location = @Location, Debt = @Debt, LastState = @LastState, Greeting = @Greeting, Grounded = @Grounded, GroundedBy = @GroundedBy, IsBotAdmin = @IsBotAdmin, LinkingKey = @LinkingKey, Description = @Description WHERE ID = @ID",
                    u);
            }
        }

        public static bool ExistsInDb(this User user, Instance db)
        {
            var rows = db.Connection.Query($"SELECT COUNT(1) as 'Count' FROM Users WHERE ID = '{user.ID}'");
            return (int) rows.First().Count > 0;
        }

        public static void RemoveFromDb(this User user, Instance db)
        {
            db.ExecuteNonQuery("DELETE FROM Users WHERE ID = @ID", user);
        }

        #endregion

        #region Settings

        public static void Save(this Setting set, Instance db)
        {
            if (set.ID == null || !ExistsInDb(set, db))
            {
                //need to insert
                db.ExecuteNonQuery(
                    "insert into settings (Alias, TelegramBotAPIKey, TelegramDefaultAdminUserId) VALUES (@Alias, @TelegramBotAPIKey, @TelegramDefaultAdminUserId)",
                    set);
                set.ID = db.Connection.Query<int>("SELECT ID FROM Settings WHERE Alias = @Alias", set).First();
            }
            else
            {
                db.ExecuteNonQuery(
                    "UPDATE settings SET Alias = @Alias, TelegramBotAPIKey = @TelegramBotAPIKey, TelegramDefaultAdminUserId = @TelegramDefaultAdminUserId WHERE ID = @ID",
                    set);
            }
        }

        public static bool ExistsInDb(this Setting set, Instance db)
        {
            var rows = db.Connection.Query($"SELECT COUNT(1) as 'Count' FROM settings WHERE ID = '{set.ID}'");
            return (int) rows.First().Count > 0;
        }

        public static void RemoveFromDb(this Setting set, Instance db)
        {
            db.ExecuteNonQuery("DELETE FROM settings WHERE ID = @ID", set);
        }

        /// <summary>
        /// Adds a field to the settings table, if needed
        /// </summary>
        /// <param name="set">the current settings loaded</param>
        /// <param name="db">Instance of the database</param>
        /// <param name="field">Name of the field you need</param>
        /// <returns>Whether or not the field was missing / was added</returns>
        public static bool AddField(this Setting set, Instance db, string field)
        {
            if (db.Connection.State != ConnectionState.Open)
                db.Connection.Open();
            //verify settings exist
            var columns = new SQLiteCommand("PRAGMA table_info(settings)", db.Connection).ExecuteReader();
            var settingExists = false;
            while (columns.Read())
            {
                if (String.Equals(columns[1].ToString(), field))
                    settingExists = true;
            }

            if (!settingExists)
            {
                new SQLiteCommand($"ALTER TABLE settings ADD COLUMN {field} TEXT DEFAULT '';", db.Connection)
                    .ExecuteNonQuery();
                return true;
            }

            return false;
        }


        /// <summary>
        /// Returns the requested field from settings.
        /// </summary>
        /// <param name="set"></param>
        /// <param name="db"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public static string GetString(this Setting set, Instance db, string field)
        {
            return
                db.Connection.Query<string>($"select {field} from settings where Alias = '{set.Alias}'")
                    .FirstOrDefault();
        }

        public static void SetString(this Setting set, Instance db, string field, string value)
        {
            new SQLiteCommand($"Update settings set {field} = '{value}' WHERE Alias = '{set.Alias}'", db.Connection)
                .ExecuteNonQuery();
        }

        #endregion

        public static void ExecuteNonQuery(this Instance db, string commandText, object param = null)
        {
            // Ensure we have a connection
            if (db.Connection == null)
            {
                throw new NullReferenceException(
                    "Please provide a connection");
            }

            // Ensure that the connection state is Open
            if (db.Connection.State != ConnectionState.Open)
            {
                db.Connection.Open();
            }

            // Use Dapper to execute the given query
            db.Connection.Execute(commandText, param);
        }
    }
}
