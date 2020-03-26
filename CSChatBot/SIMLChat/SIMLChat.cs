using DB;
using DB.Extensions;
using DB.Models;
using ModuleFramework;
using Syn.Bot.Siml;
using Syn.Bot.Siml.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Telegram.Bot;

namespace SIMLChat
{
    /// <summary>
    /// A sample module for CSChatBot
    /// </summary>
    [ModuleFramework.Module(Author = "Para", Name = "SIML Chat", Version = "1.0")]
    public class SIMLChat
    {
        private Dictionary<long, BotUser> _chats = new Dictionary<long, BotUser>();
        public SIMLChat(Instance db, Setting settings, TelegramBotClient bot)
        {
            var simlBot = new SimlBot();
            foreach (var simlFile in Directory.GetFiles("SimlFiles", "*.siml", SearchOption.TopDirectoryOnly))
            {
                var simlDocument = XDocument.Load(simlFile);
                simlBot.Import(simlDocument);
            }
            foreach (var simlFile in Directory.GetFiles("SimlFiles\\Settings", "*.siml", SearchOption.TopDirectoryOnly))
            {
                var simlDocument = XDocument.Load(simlFile);
                simlBot.Import(simlDocument);
            }
            
            simlBot.Learning += SimlBot_Learning;
            simlBot.Memorizing += simlBot_Memorizing;
            
            var me = bot.GetMeAsync().Result.Id;
            //do your initialization here.  this is done before the bot begins receiving updates.
            //DO NOT run  bot.StartReceiving();
            //This is run in the main bot already.

            //You can subscribe to updates.This is a simple echo
            bot.OnUpdate += (sender, args) =>
            {
                try
                {

                    if (args.Update?.Message?.Chat == null || args.Update?.Message?.Text == null) return;
                    var text = args.Update.Message.Text;
                    if (text.StartsWith("!") || text.StartsWith("/")) return;
                    BotUser bU;
                    if (args.Update.Message.Chat.Type == Telegram.Bot.Types.Enums.ChatType.Private)
                    {

                    }
                    else
                    {
                        if (!text.ToLower().StartsWith("seras") && args.Update.Message.ReplyToMessage?.From?.Id != me) return;
                        text = text.Replace("seras", "").Trim();
                        var g = db.GetGroupById(args.Update.Message.Chat.Id);
                        var ai = g.GetSetting<bool>("chatai", db, false);
                        //if (!ai) return;

                        //if (_chats.ContainsKey(g.GroupId))
                        //    bU = _chats[g.GroupId];
                        //else
                        //{
                        //    bU = simlBot.CreateUser();
                        //    _chats.Add(g.GroupId, bU);
                        //}
                    }


                    var u = db.GetUserById(args.Update.Message.From.Id);
                    if (_chats.ContainsKey(u.UserId))
                        bU = _chats[u.UserId];
                    else
                    {
                        bU = simlBot.CreateUser(u.UserId.ToString());
                        if (Directory.Exists(Path.Combine("SimlFiles", bU.ID)))
                        {
                            var filePath = Path.Combine("SimlFiles", bU.ID, "Memorized.siml");
                            var memorizedDocument = XDocument.Load(filePath);
                            simlBot.Import(memorizedDocument, bU);
                        }
                        _chats.Add(u.UserId, bU);
                    }

                    var request = new ChatRequest(text, bU);
                    var result = simlBot.Chat(request);
                    bot.SendTextMessageAsync(args.Update.Message.Chat.Id, result.BotMessage);
                    //process text with SIML
                }
                catch
                {
                    //ignored
                }
                finally
                {

                }

            };

            //Or other things, like callback queries (inline buttons)
            //bot.OnCallbackQuery += (sender, args) =>
            //{
            //    bot.SendTextMessageAsync(args.CallbackQuery.From.Id, args.CallbackQuery.Data);
            //};
        }

        private void simlBot_Memorizing(object sender, MemorizingEventArgs e)
        {
            var filePath = Path.Combine("SimlFiles", e.User.ID, "Memorized.siml");
            e.Document.Save(filePath);
        }

        private void SimlBot_Learning(object sender, Syn.Bot.Siml.Events.LearningEventArgs e)
        {
            var filePath = Path.Combine("SimlFiles", "Learned.siml");
            e.Document.Save(filePath);
        }

        [ChatCommand(Triggers = new[] { "togglechat" }, DontSearchInline = true, InGroupOnly = true, HelpText = "Toggles SIML Chat API")]
        public static CommandResponse Test(CommandEventArgs args)
        {
            var g = args.DatabaseInstance.GetGroupById(args.Message.Chat.Id);
            var ai = !g.GetSetting<bool>("chatai", args.DatabaseInstance, false);
            g.SetSetting<bool>("chatai", args.DatabaseInstance, false, ai);
            return new CommandResponse("chatai: " + ai);
        }
    }
}
