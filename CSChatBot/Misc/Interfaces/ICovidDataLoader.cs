using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misc.Interfaces
{
    public interface ICovidDataLoader
    {
        List<RegionData> LoadData();
        RegionData GetStats(string input);
    }
}
