using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misc.Models
{

}

public class CovidStats
{
    public string objectIdFieldName { get; set; }
    public Uniqueidfield uniqueIdField { get; set; }
    public string globalIdFieldName { get; set; }
    public string geometryType { get; set; }
    public Spatialreference spatialReference { get; set; }
    public Field[] fields { get; set; }
    public Feature[] features { get; set; }
}

public class Uniqueidfield
{
    public string name { get; set; }
    public bool isSystemMaintained { get; set; }
}

public class Spatialreference
{
    public int wkid { get; set; }
    public int latestWkid { get; set; }
}

public class Field
{
    public string name { get; set; }
    public string type { get; set; }
    public string alias { get; set; }
    public string sqlType { get; set; }
    public int length { get; set; }
    public object domain { get; set; }
    public object defaultValue { get; set; }
}

public class Feature
{
    public RegionData attributes { get; set; }
}

public class RegionData
{
    public string Country { get; set; }
    public string State { get; set; }
    public int TotalCases { get; set; }
    public int TotalRecovered { get; set; }
    public int TotalDeaths { get; set; }
    public int ActiveCases { get; set; }
    public int OBJECTID { get; set; }
    public float TotalCasesPerMillion { get; set; }
    public int SeriousCases { get; set; }
    public int NewDeaths { get; set; }
    public int NewCases { get; set; }
    public string Source { get; set; }
}
