using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdleKittenAuto.WebPage;

namespace IdleKittenAuto
{
    //public static class Buildings
    //{      
        //public static Dictionary<string, Building> Dictionary = new Dictionary<string, Building>()
        //{
        //    {
        //        "catnipfield", new Building()
        //        {
        //            Name = "Catnip Field",
        //            Count = 0,
        //            Ratio = 1.12,
        //            BaseRequirements = new Dictionary<string, double>() { { "catnip", 10} },
        //            Requirements = new Dictionary<string, double>() { { "catnip", 10 } },
        //            Produces = new List<Effect>()
        //            {
        //                new Effect()
        //                {
        //                    Resource = new Resource()
        //                    {
        //                        Name = "catnip",
        //                        PerTick = new Rate()
        //                        {
        //                            Positive = true,
        //                            Delta = (0.125 * 5)
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    },
        //    {
        //        "hut", new Building()
        //        {
        //            Name = "Hut",
        //            Count = 0,
        //            Ratio = 2.25,
        //            BaseRequirements = new Dictionary<string, double> { { "wood", 5 } },
        //            Requirements = new Dictionary<string, double> { { "wood", 5 } },
        //            Produces = new List<Effect>()
        //            {
        //                new Effect()
        //                {
        //                    Resource = new Resource()
        //                    {
        //                        Name = "kittens",
        //                        MaxAmount = 2
        //                    }
        //                },
        //                new Effect()
        //                {
        //                    Resource = new Resource()
        //                    {
        //                        Name = "catpower",
        //                        MaxAmount = 75
        //                    }
        //                }
        //            }
        //        }
        //    }
        //};
    //}

    public class Building
    {
        public string Name { get; set; }
        public double Count { get; set; }
        public double Ratio { get; set; }
        public Dictionary<string, double> BaseRequirements { get; set; }
        public Dictionary<string, double> Requirements { get; set; }
        public Effect Produces { get; set; } //TODO: Maybe change this var name
    }


    public class Effect
    {
        public Dictionary<string, Resource> Resources { get; set; }
        public Rate RatePercentage { get; set; }
    }

    //public class ResourceProduction : Effect
    //{
    //    public ResourceProduction(string Name, bool Positive, double Delta)
    //    {
    //        this.Name = Name;
    //        PerSecond = new Rate()
    //        {
    //            Positive = Positive,
    //            Delta = Delta * 5
    //        };
    //    }
    //}

    //public class ResourceBonus : Effect
    //{
    //    public ResourceBonus(string Name, double PercentChange, bool DeltaIsPos)
    //    {
    //        this.Name = Name;
    //        this.PercentChange = PercentChange;
    //        this.DeltaIsPos = DeltaIsPos;
    //    }

    //}

    //public class StorageIncrease : Effect
    //{
    //    public StorageIncrease(string Name, int AmountIncrease)
    //    {
    //        this.Name = Name;
    //        this.AmountIncrease = AmountIncrease;
    //    }
    //}

}

