using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misc.Models
{
    public class CountryStats
    {
        public string Country { get; set; }
        public int TotalCases { get; set; }
        public int NewCases { get; set; }
        public int TotalDeaths { get; set; }
        public int NewDeaths { get; set; }
        public int TotalRecovered { get; set; }
        public int ActiveCases { get; set; }
        public int SeriousCases { get; set; }
        public float TotalCasesPerMillion { get; set; }
    }
}
