using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdleKittenAuto
{
    public class Job
    {
        public string Title { get; set; }
        public string Prerequesite { get; set; }
        public string Resource { get; set; }
        public double PerTick { get; set; }
        public double Count { get; set; }
    }

    public static class JobDictionary
    {
        public static Dictionary<string, Job> Dictionary = new Dictionary<string, Job>()
        {
            {
                "unemployed", new Job()
                {
                    Title = "unemployed",
                    Count = 0
                }
            },
            {
                "woodcutter", new Job()
                {
                    Title = "woodcutter",
                    Resource = "wood",
                    PerTick = 0.015,
                    Count = 0
                }
            },
            {
                "farmer", new Job()
                {
                    Title = "farmer",
                    Prerequesite = "agriculture",
                    Resource = "catnip",
                    PerTick = 1.00,
                    Count = 0
                }
            },
            {
                "scholar", new Job()
                {
                    Title = "scholar",
                    Prerequesite = "library",
                    Resource = "science",
                    PerTick = 0.05,
                    Count = 0
                }
            },
            {
                "hunter", new Job()
                {
                    Title = "hunter",
                    Prerequesite = "archery",
                    Resource = "catpower",
                    PerTick = 0.06,
                    Count = 0
                }
            },
            {
                "miner", new Job()
                {
                    Title = "miner",
                    Prerequesite = "mine",
                    Resource = "minerals",
                    PerTick = 0.05,
                    Count = 0
                }
            }
        };
    }
}
