using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misc
{
    public static class RandoFactGenerator
    {
        internal static Random R = new Random();
        private static List<PathString> Set1, Set2, Set3, Set4;
        public static void Init()
        {
            Set1 = new List<PathString>
            {
                new PathString("the {0} equinox", "fall", "spring"),
                new PathString("the {0} {1}", "winter", "summer") { Options2 = new [] { "solstice", "olympics" } },
                new PathString("the {0} {1}", "earliest", "latest") { Options2 = new [] { "sunrise", "sunset" } },
                new PathString("daylight {0} time", "saving", "savings"),
                new PathString("leap {0}", "day", "year"),
                new PathString("Easter"),
                new PathString("the {0} Moon", "Harvest", "Blood", "Super"),
                new PathString("Toyota Truck Month"),
                new PathString("Shark Week")
            };


            Set2 = new List<PathString>
            {
                new PathString("happens {0} every year", "earlier", "later", "at the wrong time"),
                new PathString("drifts out of sync with the {0}", "sun", "moon", "zodiac", "Gregorian calendar", "Mayan calendar", "Lunar calendar", "iPhone calendar", "atomic clock in Colorado"),
                new PathString("might {0} this year", "not happen", "happen twice")
            };

            Set3 = new List<PathString>
            {
                new PathString("time zone legislation in {0}", "Indiana", "Arizona", "Russia"),
                new PathString("a decree by the Pope in the 1500s"),
                new PathString("{0} of the {1}", "precession", "liberation", "nutation", "libation", "eccentricity", "obliquity")
                {
                    Options2 = new []{"moon", "sun", "Earth's axis", "equator", "Prime Meridian", "International Date Line", "Mason-Dixon Line"}
                },
                new PathString("magnetic field reversal"),
                new PathString("an arbitrary decision by {0}", "Benjamin Franklin", "Isaac Newton", "FDR")
            };

            Set4 = new List<PathString>
            {
                new PathString("it was even more extreme during the {0}", "Bronze Age", "Ice Age", "Cretaceous", "1990s"),
                new PathString("it causes a predictable increase in car accidents"),
                new PathString("that's why we have leap seconds"),
                new PathString("scientists are really worried"),
                new PathString("there's a proposal to fix it, but it {0}", "will never happen", "actually makes things worse", "is stalled in Congress", "might be unconstitutional"),
                new PathString("it's getting worse and no one knows why")
            };
        }

        public static string Get()
        {
            return $"Did you know that {Set1[R.Next(Set1.Count)].Text} {Set2[R.Next(Set2.Count)].Text} because of {Set3[R.Next(Set3.Count)].Text}?  Apparently {Set4[R.Next(Set4.Count)].Text}.";
        }
    }

    

    internal class PathString
    {
        public string BaseText { get; set; }
        public string Text
        {
            get
            {
                if (Options2.Any())
                {
                    return String.Format(BaseText, RandomOption1, RandomOption2);
                }
                if (Options1.Any())
                {
                    return String.Format(BaseText, RandomOption1);
                }
                return BaseText;
            }
        }

        public string[] Options1 { get; set; } = new string[0];
        public string[] Options2 { get; set; } = new string[0];

        public PathString(string text, params string[] options)
        {
            BaseText = text;
            Options1 = options;
        }

        public PathString(string text)
        {
            BaseText = text;
        }

        private string RandomOption1
        {
            get
            {
                return Options1[RandoFactGenerator.R.Next(Options1.Length)];
            }
        }

        private string RandomOption2
        {
            get
            {
                return Options2[RandoFactGenerator.R.Next(Options2.Length)];
            }
        }
    }

}
