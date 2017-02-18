using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Models
{
    public partial class Group
    {
        /// <summary>
        /// The database id of the group
        /// </summary>
        public int? ID { get; internal set; }
        /// <summary>
        /// The groups name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The groups Id
        /// </summary>
        public long GroupId { get; set; }
        /// <summary>
        /// The groups @username, for public groups
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// Number of users in the group
        /// </summary>
        public int MemberCount { get; set; }
    }
}
