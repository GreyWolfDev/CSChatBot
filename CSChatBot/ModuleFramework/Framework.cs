using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB;
using DB.Models;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace ModuleFramework
{
    [AttributeUsage(AttributeTargets.Class)]
    public class Module : Attribute
    {
        public string Author { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class ChatCommand : Attribute
    {
        /// <summary>
        /// What triggers the command? starts with ! or /
        /// </summary>
        public string[] Triggers { get; set; }
        /// <summary>
        /// Only bot admins can use (moderators)
        /// </summary>
        public bool BotAdminOnly { get; set; } = false;
        /// <summary>
        /// Only group admins can use this
        /// </summary>
        public bool GroupAdminOnly { get; set; } = false;
        /// <summary>
        /// Only developers / bot owner (you) can use this
        /// </summary>
        public bool DevOnly { get; set; } = false;
        /// <summary>
        /// Command can only be used in group
        /// </summary>
        public bool InGroupOnly { get; set; } = false;
        /// <summary>
        /// Command can only be used in Private (private info, or LARGE messages / spam
        /// </summary>
        public bool InPrivateOnly { get; set; } = false;
        /// <summary>
        /// Sets the help text for this command
        /// </summary>
        public string HelpText { get; set; }
    }

    public class CommandResponse
    {
        /// <summary>
        /// The text to send
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// Where to reply.  Private = in PM, Public = The chat the message is from
        /// </summary>
        public ResponseLevel Level { get; set; }
        public ReplyMarkup Markup { get; set; }
        public ParseMode ParseMode { get; set; }
        /// <summary>
        /// Sends a response through the bot
        /// </summary>
        /// <param name="msg">The text to send</param>
        /// <param name="level">Where to reply.  Private = in PM, Public = The chat the message is from</param>
        /// <param name="replyMarkup">Reply markup.  Optional</param>
        /// <param name="parseMode">How the text should be parsed</param>

        public CommandResponse(string msg, ResponseLevel level = ResponseLevel.Public, ReplyMarkup replyMarkup = null, ParseMode parseMode = ParseMode.Default)
        {
            Text = msg;
            Level = level;
            Markup = replyMarkup;
            ParseMode = parseMode;
        }
    }

    public class CommandEventArgs
    {
        public Instance DatabaseInstance { get; set; }
        public User SourceUser { get; set; }
        public string Parameters { get; set; }
        public string Target { get; set; } //groupid, userid
        public ModuleMessenger Messenger { get; set; }
        public TelegramBotClient Bot { get; set; }
    }

    public delegate void MessageSentEventHandler(object sender, MessageSentEventArgs e);

    public class ModuleMessenger
    {
        public event EventHandler MessageSent;

        protected virtual void OnMessageSent(MessageSentEventArgs e)
        {
            EventHandler handler = MessageSent;
            handler?.Invoke(this, e);
        }

        public void SendMessage(MessageSentEventArgs args)
        {
            OnMessageSent(args);
        }
    }

    public class MessageSentEventArgs : EventArgs
    {
        public string Target { get; set; }
        public CommandResponse Response { get; set; }
    }
    
    /// <summary>
    /// Forces a response level
    /// </summary>
    public enum ResponseLevel
    {
        Public, Private
    }
}
