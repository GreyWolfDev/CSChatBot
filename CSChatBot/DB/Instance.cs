using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using Dapper;
using DB.Models;
using Logger;

namespace DB
{
    /// <summary>
    /// Instance of a bot database
    /// </summary>
    public class Instance
    {
        public SQLiteConnection Connection;

        public Log Log;

        /// <summary>
        /// Initializes an instance of a sqlite database for the bot
        /// </summary>
        /// <param name="dbpath">path to the database file</param>
        /// <param name="logpath">path to log file for the database</param>
        public Instance(string dbpath, string logpath)
        {
            Log = new Log(logpath);
            if (!File.Exists(dbpath))
            {
                CreateDatabase(dbpath);
            }
            else
            {
                Connection = new SQLiteConnection($"Data Source={dbpath};Version=3;");
                Connection.Open();
            }
            Log.WriteLine("Database connection succeeded");
        }

        internal void CreateDatabase(string path)
        {
            Log.WriteLine("Database not found, creating...", LogLevel.Warn);//, fileName: "db.log");
            SQLiteConnection.CreateFile(path);

            Connection = new SQLiteConnection($"Data Source={path};Version=3;");
            Connection.Open();
            Log.WriteLine("Creating user table...");//, fileName: "db.log");

            new SQLiteCommand(
                @"create table users (ID INTEGER PRIMARY KEY AUTOINCREMENT, 
                    Name TEXT NOT NULL, UserId INTEGER NOT NULL, UserName TEXT, FirstSeen TEXT, LastHeard TEXT, 
                    Points INTEGER DEFAULT 0, Location TEXT, Debt INTEGER DEFAULT 0, 
                    LastState TEXT, Greeting TEXT, Grounded INTEGER DEFAULT 0, GroundedBy TEXT, 
                    IsBotAdmin INTEGER DEFAULT 0, LinkingKey TEXT, Description TEXT)", Connection)
                .ExecuteNonQuery();
            Log.WriteLine("User table created successfully");//, fileName: "db.log");
            
            Log.WriteLine("Creating settings table...");
            new SQLiteCommand(
                @"create table settings (ID INTEGER PRIMARY KEY AUTOINCREMENT, 
                    Alias TEXT UNIQUE NOT NULL, TelegramBotAPIKey TEXT, TelegramDefaultAdminUserId TEXT, WWApiKey TEXT)", Connection)
                .ExecuteNonQuery();
            Log.WriteLine("Settings table created successfully");

            new SQLiteCommand(@"create table chatgroup (ID INTEGER PRIMARY KEY AUTOINCREMENT, GroupId INTEGER UNIQUE NOT NULL, Name TEXT, UserName TEXT, MemberCount INTEGER)", Connection).ExecuteNonQuery();
        }

        public IEnumerable<Setting> Settings => Connection.Query<Setting>("select * from settings");

        public IEnumerable<User> Users => Connection.Query<User>("select * from users");

        public IEnumerable<Group> Groups => Connection.Query<Group>("select * from chatgroup");

        public User GetUser(int ID)
        {
            return Connection.Query<User>($"select * from users where ID = {ID}").FirstOrDefault();
        }

        [Obsolete("Do not search by name, Telegram allows multiple users with same name")]
        public User GetUserByName(string nick)
        {
            return Connection.Query<User>($"select * from users where Name = '{nick}'").FirstOrDefault();
        }

        public User GetUserById(int Id)
        {
            return Connection.Query<User>($"select * from users where UserId = {Id}").FirstOrDefault();
        }

        public Group GetGroup(int ID)
        {
            return Connection.Query<Group>($"select * from chatgroup where ID = {ID}").FirstOrDefault();
        }


        public Group GetGroupById(long Id)
        {
            return Connection.Query<Group>($"select * from chatgroup where GroupId = {Id}").FirstOrDefault();
        }
    }
}
