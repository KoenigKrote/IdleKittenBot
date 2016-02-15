using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdleKittenAuto.WebPage;

namespace IdleKittenAuto
{
    //The intent of this class is to give the program an object to work towards, and
    //allocate resources and workers towards that.
    public static class Objective
    {
        public static Building Building { get; set; }
        public static List<KeyValuePair<string, double>> ResourceGoal = new List<KeyValuePair<string, double>>();
        public static double KittenCount { get; set; }

        public static void determineObjective()
        {
            Resource catnip = Helper.getResource("catnip");
            if (catnip.PerTick.Positive != true || catnip.PerTick.Delta < 8)
            {
                Building = Buildings.CatnipField;
                getResourceGoal();
                return;
            }

            Resource kittens = Helper.getResource("kittens");
            if (kittens == null || kittens.MaxAmount < 4)
            {
                Building = Buildings.Hut;
                getResourceGoal();
                return;
            }

            //TODO: Rewrite this to check the whole dictionary for any available building with count 0
            if (Buildings.Library.Available && Buildings.Library.Count == 0)
            {
                Building = Buildings.Library;
                getResourceGoal();
                return;
            }
        }

        public static void getResourceGoal()
        {
            try
            {
                ResourceGoal.Clear();
                foreach (var item in Building.Requirements)
                {
                    double Percentage = Helper.getResource(item.Key).Amount / item.Value;
                    Percentage = 1 - Percentage;
                    ResourceGoal.Add(new KeyValuePair<string, double>(item.Key, Percentage));
                }

            }
            catch
            {

            }
        }
    }
}
