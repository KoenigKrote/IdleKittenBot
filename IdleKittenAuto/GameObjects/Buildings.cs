using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdleKittenAuto.WebPage;

namespace IdleKittenAuto
{
    public static class Buildings
    {
        public static Building CatnipField = new Building()
        {
            Name = "Catnip Field",
            Count = 0,
            Ratio = 1.12,
            Available = true,
            BaseRequirements = new Dictionary<string, double>() { { "catnip", 10 } },
            Requirements = new Dictionary<string, double>() { { "catnip", 10 } },
            Produces = new Effect()
            {
                Resources = new Dictionary<string, Resource>()
                {
                    { "catnip", new Resource()
                    {
                        Name = "catnip",
                        PerTick = new Rate()
                        {
                            Positive = true,
                            Delta = (0.125 * 5)
                        }
                    }
                    }
                }
            }
        };

        public static Building Pasture = new Building()
        {
            Name = "Pasture",
            Count = 0,
            Ratio = 1.15,
            Available = false,
            BaseRequirements = new Dictionary<string, double>() { { "catnip", 100 }, { "wood", 10 } },
            Requirements = new Dictionary<string, double>() { { "catnip", 100 }, { "wood", 10 } },
            Produces = new Effect()
            {
                RatePercentage = new Rate()
                {
                    Name = "catnip",
                    Positive = false,
                    Delta = 0.05
                }
            }
        };

        public static Building Hut = new Building()
        {
            Name = "Hut",
            Count = 0,
            Ratio = 2.50,
            Available = false,
            BaseRequirements = new Dictionary<string, double> { { "wood", 5 } },
            Requirements = new Dictionary<string, double> { { "wood", 5 } },
            Produces = new Effect()
            {
                Resources = new Dictionary<string, Resource>()
                {
                    {
                        "kittens", new Resource()
                        {
                            Name = "kittens",
                            MaxAmount = 2
                        }
                    },
                    {
                        "catpower", new Resource()
                        {
                            Name = "catpower",
                            MaxAmount = 75
                        }
                    }
                }
            }
        };

        public static Building Library = new Building()
        {
            Name = "Library",
            Count = 0,
            Ratio = 1.15,
            Available = false,
            BaseRequirements = new Dictionary<string, double> { { "wood", 25 } },
            Requirements = new Dictionary<string, double> { { "wood", 25 } },
            Produces = new Effect()
            {
                Resources = new Dictionary<string, Resource>()
                {
                    {
                        "science", new Resource()
                        {
                            Name = "science",
                            MaxAmount = 250
                        }
                    },
                    {
                        "culture", new Resource()
                        {
                            Name = "culture",
                            MaxAmount = 10
                        }
                    }
                },
                RatePercentage = new Rate()
                {
                    Name = "science",
                    Positive = true,
                    Delta = 0.08
                }
            }
        };
    }


    public class Building
    {
        public string Name { get; set; }
        public double Count { get; set; }
        public double Ratio { get; set; }
        public bool Available { get; set; }
        public Dictionary<string, double> BaseRequirements { get; set; }
        public Dictionary<string, double> Requirements { get; set; }
        public Effect Produces { get; set; } //TODO: Maybe change this var name
    }


    public class Effect
    {
        public Dictionary<string, Resource> Resources { get; set; }
        public Rate RatePercentage { get; set; }
    }
}

