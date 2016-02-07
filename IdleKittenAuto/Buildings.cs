using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdleKittenAuto.WebPage;

namespace IdleKittenAuto
{
    public class BuildingList
    {
        public Building CatnipField = new Building()
        {
            Name = "Catnip Field",
            Count = 0,
            Ratio = 1.12,
            BaseRequirements = new Dictionary<string, double>() { { "catnip", 10} },
            Requirements = new Dictionary<string, double>() { { "catnip", 10 } },
            Produces = new List<Effects>()
            {
                new ResourceProduction("catnip", true, 0.125)
            }
        };

        public Building Hut = new Building()
        {
            Name = "Hut",
            Count = 0,
            Ratio = 2.25,
            BaseRequirements = new Dictionary<string, double> { { "wood", 5 } },
            Requirements = new Dictionary<string, double> { { "wood", 5 } },
            Produces = new List<Effects>()
            {
                new StorageIncrease("kittens", 2),
                new StorageIncrease("catpower", 75)
            }
        };
    }

    public class Building
    {
        public string Name { get; set; }
        public double Count { get; set; }
        public double Ratio { get; set; }
        public Dictionary<string, double> BaseRequirements { get; set; }
        public Dictionary<string, double> Requirements { get; set; }
        public List<Effects> Produces { get; set; } //TODO: Maybe change this var name
    }

    public class Effects
    {
        public string Name { get; set; }
        public Rate PerSecond { get; set; }
        public double PercentChange { get; set; }
        public bool DeltaIsPos { get; set; }
        public int AmountIncrease { get; set; }
    }

    public class ResourceProduction : Effects
    {
        public ResourceProduction(string Name, bool Positive, double Delta)
        {
            this.Name = Name;
            PerSecond = new Rate()
            {
                Positive = Positive,
                Delta = Delta * 5
            };
        }
    }

    public class ResourceBonus : Effects
    {
        public ResourceBonus(string Name, double PercentChange, bool DeltaIsPos)
        {
            this.Name = Name;
            this.PercentChange = PercentChange;
            this.DeltaIsPos = DeltaIsPos;
        }

    }

    public class StorageIncrease : Effects
    {
        public StorageIncrease(string Name, int AmountIncrease)
        {
            this.Name = Name;
            this.AmountIncrease = AmountIncrease;
        }
    }

}

