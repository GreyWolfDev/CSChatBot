using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giveaway.Models
{
    public class Giveaway
    {
        public int? ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Active { get; set; }
        public int Owner { get; set; }
        public string MessageId { get; set; }
        public string TimeStamp { get; set; }
    }
}
