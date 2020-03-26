using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Models
{
    public class Setting
    {
        /// <summary>
        /// DB Id of the setting
        /// </summary>
        public int? ID { get; set; }
        /// <summary>
        /// Settings alias, can be loaded using launch parameters
        /// </summary>
        public string Alias { get; set; }
        /// <summary>
        /// The ID of the Telegram user who will be the main admin for the bot (typically, the person running the code)
        /// </summary>
        public int TelegramDefaultAdminUserId { get; set; }
        /// <summary>
        /// Your Telegram Bot API Token
        /// </summary>
        public string TelegramBotAPIKey { get; set; }

        public string WWApiKey { get; set; }
    }
}
