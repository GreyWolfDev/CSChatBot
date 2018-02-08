using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using Dapper;
using DB.Models;
using LiteDB;
using Logger;

namespace DB
{
    /// <summary>
    /// Instance of a bot database
    /// </summary>
    public class Instance
    {
        public SQLiteConnection Connection;
        public LiteDatabase Database;
        public Log Log;

        /// <summary>
        /// Initializes an instance of a sqlite database for the bot
        /// </summary>
        /// <param name="dbpath">path to the database file</param>
        /// <param name="logpath">path to log file for the database</param>
        public Instance(string dbpath, string logpath)
        {
            Log = new Log(logpath);
            var log = new LiteDB.Logger(LiteDB.Logger.ERROR | LiteDB.Logger.COMMAND, (s) => Log.WriteLine(s));
            Database = new LiteDatabase(dbpath, log: log);
            
            if (File.Exists(dbpath.Replace(".db",".sqlite")))
            {
                //old database exists, lets copy it to the new one
                Connection = new SQLiteConnection($"Data Source={dbpath.Replace(".db", ".sqlite")};Version=3;");
                Connection.Open();
                var uc = Database.GetCollection<User>();
                foreach (var u in Users)
                    uc.Upsert(u);
                var sc = Database.GetCollection<Setting>();
                foreach (var s in Settings)
                    sc.Upsert(s);
                var gc = Database.GetCollection<Group>();
                foreach (var g in Groups)
                    gc.Upsert(g);
                Log.WriteLine("Old Database copied to new Database");
            }
            
            Log.WriteLine("Database connection succeeded");
        }

        //internal void CreateDatabase(string path)
        //{
        //    Log.WriteLine("Database not found, creating...", LogLevel.Warn);//, fileName: "db.log");
        //    SQLiteConnection.CreateFile(path);

        //    Connection = new SQLiteConnection($"Data Source={path};Version=3;");
        //    Connection.Open();
        //    Log.WriteLine("Creating user table...");//, fileName: "db.log");

        //    new SQLiteCommand(
        //        @"create table users (ID INTEGER PRIMARY KEY AUTOINCREMENT, 
        //            Name TEXT NOT NULL, UserId INTEGER NOT NULL, UserName TEXT, FirstSeen TEXT, LastHeard TEXT, 
        //            Points INTEGER DEFAULT 0, Location TEXT, Debt INTEGER DEFAULT 0, 
        //            LastState TEXT, Greeting TEXT, Grounded INTEGER DEFAULT 0, GroundedBy TEXT, 
        //            IsBotAdmin INTEGER DEFAULT 0, LinkingKey TEXT, Description TEXT)", Connection)
        //        .ExecuteNonQuery();
        //    Log.WriteLine("User table created successfully");//, fileName: "db.log");

        //    Log.WriteLine("Creating settings table...");
        //    new SQLiteCommand(
        //        @"create table settings (ID INTEGER PRIMARY KEY AUTOINCREMENT, 
        //            Alias TEXT UNIQUE NOT NULL, TelegramBotAPIKey TEXT, TelegramDefaultAdminUserId TEXT)", Connection)
        //        .ExecuteNonQuery();
        //    Log.WriteLine("Settings table created successfully");

        //    new SQLiteCommand(@"create table chatgroup (ID INTEGER PRIMARY KEY AUTOINCREMENT, GroupId INTEGER UNIQUE NOT NULL, Name TEXT, UserName TEXT, MemberCount INTEGER)", Connection).ExecuteNonQuery();
        //}

        public IEnumerable<Setting> Settings => Database.GetCollection<Setting>().FindAll();

        public IEnumerable<User> Users => Database.GetCollection<User>().FindAll();

        public IEnumerable<Group> Groups => Database.GetCollection<Group>().FindAll();

        public User GetUser(int ID)
        {
            return Database.GetCollection<User>().FindOne(x => x.ID == ID);
        }

        [Obsolete("Do not search by name, Telegram allows multiple users with same name")]
        public User GetUserByName(string nick)
        {
            return Database.GetCollection<User>().FindOne(x => x.Name == nick);
        }

        public User GetUserById(int Id)
        {
            return Database.GetCollection<User>().FindOne(x => x.UserId == Id);
        }

        public Group GetGroup(int ID)
        {
            return Database.GetCollection<Group>().FindOne(x => x.ID == ID);
        }


        public Group GetGroupById(long Id)
        {
            return Database.GetCollection<Group>().FindOne(x => x.GroupId == Id);
        }
    }
}
