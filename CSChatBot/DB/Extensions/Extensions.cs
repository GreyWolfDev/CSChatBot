using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DB.Models;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using User = DB.Models.User;
using LiteDB;

namespace DB.Extensions
{
    public static class Extensions
    {
        #region Users

        public static void Save(this User u, Instance db)
        {
            db.Database.GetCollection<User>().Upsert(u);
        }

        //public static bool ExistsInDb(this User user, Instance db)
        //{
        //    var rows = db.Connection.Query($"SELECT COUNT(1) as 'Count' FROM Users WHERE ID = '{user.ID}'");
        //    return (int)rows.First().Count > 0;
        //}

        //public static void RemoveFromDb(this User user, Instance db)
        //{
        //    db.ExecuteNonQuery("DELETE FROM Users WHERE ID = @ID", user);
        //}

        /// <summary>
        /// Gets a user setting from the database
        /// </summary>
        /// <typeparam name="T">The type the setting should be (bool, int, string)</typeparam>
        /// <param name="user">What user the setting comes from</param>
        /// <param name="field">The name of the setting</param>
        /// <param name="db">The database instance</param>
        /// <param name="def">The default value for the field</param>
        /// <returns></returns>
        public static T GetSetting<T>(this User user, string field, Instance db, object def)
        {
            //if (db.Connection.State != ConnectionState.Open)
            //    db.Connection.Open();
            ////verify settings exist
            //var columns = new SQLiteCommand("PRAGMA table_info(users)", db.Connection).ExecuteReader();
            //var t = default(T);

            //while (columns.Read())
            //{
            //    if (String.Equals(columns[1].ToString(), field))
            //    {
            //        var result = new SQLiteCommand($"select {field} from users where ID = {user.ID}", db.Connection).ExecuteScalar();
            //        if (t != null && t.GetType() == typeof(bool))
            //        {
            //            result = (result.ToString() == "1"); //make it a boolean value
            //        }
            //        return (T)result;
            //    }
            //}
            //var type = "BLOB";
            //if (t == null)
            //    type = "TEXT";
            //else if (t.GetType() == typeof(int))
            //    type = "INTEGER";
            //else if (t.GetType() == typeof(bool))
            //    type = "INTEGER";

            //new SQLiteCommand($"ALTER TABLE users ADD COLUMN {field} {type} DEFAULT {(type == "INTEGER" ? def : $"'{def}'")};", db.Connection)
            //    .ExecuteNonQuery();
            return (T)def;

        }

        /// <summary>
        /// Sets a user setting to the database
        /// </summary>
        /// <typeparam name="T">The type the setting should be (bool, int, string)</typeparam>
        /// <param name="user">What user the setting comes from</param>
        /// <param name="field">The name of the setting</param>
        /// <param name="db">The database instance</param>
        /// <param name="def">The default value for the field</param>
        /// <returns></returns>
        public static bool SetSetting<T>(this User user, string field, Instance db, object def, object value)
        {
            try
            {
                if (db.Connection.State != ConnectionState.Open)
                    db.Connection.Open();
                //verify settings exist
                var columns = new SQLiteCommand("PRAGMA table_info(users)", db.Connection).ExecuteReader();
                var t = default(T);
                var type = "BLOB";
                if (t == null)
                    type = "TEXT";
                else if (t.GetType() == typeof(int))
                    type = "INTEGER";
                else if (t.GetType() == typeof(bool))
                    type = "INTEGER";
                bool settingExists = false;
                while (columns.Read())
                {
                    if (String.Equals(columns[1].ToString(), field))
                    {
                        settingExists = true;
                    }
                }
                if (!settingExists)
                {
                    new SQLiteCommand($"ALTER TABLE users ADD COLUMN {field} {type} DEFAULT {(type == "INTEGER" ? def : $"'{def}'")};", db.Connection)
                        .ExecuteNonQuery();
                }

                new SQLiteCommand($"UPDATE users set {field} = {(type == "INTEGER" ? (t != null && t.GetType() == typeof(bool)) ? (bool)value ? "1" : "0" : value : $"'{value}'")} where ID = {user.ID}", db.Connection).ExecuteNonQuery();

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        #endregion

        #region Groups

        public static void Save(this Group u, Instance db)
        {
            if (u.ID == null || !ExistsInDb(u, db))
            {
                //need to insert
                db.ExecuteNonQuery(
                    "insert into chatgroup (GroupId, Name, UserName, MemberCount) VALUES (@GroupId, @Name, @UserName, @MemberCount)",
                    u);
                u.ID =
                    db.Connection.Query<int>(
                        $"SELECT ID FROM chatgroup WHERE GroupId = @GroupId", u)
                        .First();
            }
            else
            {
                db.ExecuteNonQuery(
                    "UPDATE chatgroup SET GroupId = @GroupId, Name = @Name, UserName = @UserName, MemberCount = @MemberCount WHERE ID = @ID",
                    u);
            }
        }

        public static bool ExistsInDb(this Group group, Instance db)
        {
            var rows = db.Connection.Query($"SELECT COUNT(1) as 'Count' FROM chatgroup WHERE ID = '{group.ID}'");
            return (int)rows.First().Count > 0;
        }

        public static void RemoveFromDb(this Group group, Instance db)
        {
            db.ExecuteNonQuery("DELETE FROM chatgroup WHERE ID = @ID", group);
        }

        /// <summary>
        /// Gets a group setting from the database
        /// </summary>
        /// <typeparam name="T">The type the setting should be (bool, int, string)</typeparam>
        /// <param name="group">What group the setting comes from</param>
        /// <param name="field">The name of the setting</param>
        /// <param name="db">The database instance</param>
        /// <param name="def">The default value for the field</param>
        /// <returns></returns>
        public static T GetSetting<T>(this Group group, string field, Instance db, object def)
        {
            if (db.Connection.State != ConnectionState.Open)
                db.Connection.Open();
            //verify settings exist
            var columns = new SQLiteCommand("PRAGMA table_info(chatgroup)", db.Connection).ExecuteReader();
            var t = default(T);
            while (columns.Read())
            {
                if (String.Equals(columns[1].ToString(), field))
                {
                    var result = new SQLiteCommand($"select {field} from chatgroup where ID = {group.ID}", db.Connection).ExecuteScalar();
                    if (t != null && t.GetType() == typeof(bool))
                    {
                        result = (result.ToString() == "1"); //make it a boolean value
                    }
                    return (T)result;
                }
            }
            var type = "BLOB";
            if (t == null)
                type = "TEXT";
            else if (t.GetType() == typeof(int))
                type = "INTEGER";
            else if (t.GetType() == typeof(bool))
                type = "INTEGER";
            new SQLiteCommand($"ALTER TABLE chatgroup ADD COLUMN {field} {type} DEFAULT {(type == "INTEGER" ? def : $"'{def}'")};", db.Connection)
                .ExecuteNonQuery();
            return (T)def;

        }

        /// <summary>
        /// Gets a group setting from the database
        /// </summary>
        /// <typeparam name="T">The type the setting should be (bool, int, string)</typeparam>
        /// <param name="group">What group the setting comes from</param>
        /// <param name="field">The name of the setting</param>
        /// <param name="db">The database instance</param>
        /// <param name="def">The default value for the field</param>
        /// <returns></returns>
        public static bool SetSetting<T>(this Group group, string field, Instance db, object def, object value)
        {
            try
            {
                if (db.Connection.State != ConnectionState.Open)
                    db.Connection.Open();
                //verify settings exist
                var columns = new SQLiteCommand("PRAGMA table_info(chatgroup)", db.Connection).ExecuteReader();
                var t = default(T);
                var type = "BLOB";
                if (t == null)
                    type = "TEXT";
                else if (t.GetType() == typeof(int))
                    type = "INTEGER";
                else if (t.GetType() == typeof(bool))
                    type = "INTEGER";
                bool settingExists = false;
                while (columns.Read())
                {
                    if (String.Equals(columns[1].ToString(), field))
                    {
                        settingExists = true;
                    }
                }
                if (!settingExists)
                {
                    new SQLiteCommand($"ALTER TABLE chatgroup ADD COLUMN {field} {type} DEFAULT {(type == "INTEGER" ? def : $"'{def}'")};", db.Connection)
                        .ExecuteNonQuery();
                }

                new SQLiteCommand($"UPDATE chatgroup set {field} = {(type == "INTEGER" ? (t != null && t.GetType() == typeof(bool)) ? (bool)value ? "1" : "0" : value : $"'{value}'")} where ID = {group.ID}", db.Connection).ExecuteNonQuery();

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
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
            return (int)rows.First().Count > 0;
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

        //public static string ExecuteQuery(this Instance db, string commandText, object param = null)
        //{
        //    // Ensure we have a connection
        //    if (db.Connection == null)
        //    {
        //        throw new NullReferenceException(
        //            "Please provide a connection");
        //    }

        //    // Ensure that the connection state is Open
        //    if (db.Connection.State != ConnectionState.Open)
        //    {
        //        db.Connection.Open();
        //    }
        //    var reader = db.Connection.ExecuteReader(commandText, param);
        //    var response = "";
        //    for (int i = 0; i < reader.FieldCount; i++)
        //    {
        //        response += $"{reader.GetName(i)} - ";
        //    }
        //    response += "\n";
        //    while (reader.Read())
        //    {
        //        for (int i = 0; i < reader.FieldCount; i++)
        //        {
        //            response += $"{reader[i]} - ";
        //        }
        //        response += "\n";
        //    }
        //    // Use Dapper to execute the given query
        //    return response;
        //}

        public static int ExecuteNonQuery(this Instance db, string commandText, object param = null)
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
            return db.Connection.Execute(commandText, param);
        }

        public static string ToString(this Telegram.Bot.Types.User user)
        {
            return (user.FirstName + " " + user.LastName).Trim();
        }

        #region Helpers
        public static User GetTarget(this Message message, string args, User sourceUser, Instance db)
        {
            if (message == null) return sourceUser;
            if (message?.ReplyToMessage != null)
            {
                var m = message.ReplyToMessage;
                var userid = m.ForwardFrom?.Id ?? m.From.Id;
                return db.Users.FirstOrDefault(x => x.UserId == userid) ?? sourceUser;
            }
            if (String.IsNullOrWhiteSpace(args))
            {
                return sourceUser;
            }
            //check for a user mention
            var mention = message?.Entities.FirstOrDefault(x => x.Type == MessageEntityType.Mention);
            var textmention = message?.Entities.FirstOrDefault(x => x.Type == MessageEntityType.TextMention);
            var id = 0;
            var username = "";
            if (mention != null)
                username = message.Text.Substring(mention.Offset + 1, mention.Length - 1);
            else if (textmention != null)
            {
                id = textmention.User.Id;
            }
            User result = null;
            if (!String.IsNullOrEmpty(username))
                result = db.Users.FirstOrDefault(
                    x =>
                        String.Equals(x.UserName, username,
                            StringComparison.InvariantCultureIgnoreCase));
            else if (id != 0)
                result = db.Users.FirstOrDefault(x => x.UserId == id);
            else
                result = db.Users.FirstOrDefault(
                        x =>
                            String.Equals(x.UserId.ToString(), args, StringComparison.InvariantCultureIgnoreCase) ||
                            String.Equals(x.UserName, args.Replace("@", ""), StringComparison.InvariantCultureIgnoreCase));
            return result ?? sourceUser;
        }
        #endregion
    }
}
