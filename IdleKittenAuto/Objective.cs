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
        public static double KittenCount { get; set; }

        public static void determineObjective()
        {
            Resource catnip = Helper.getResource("catnip");
            if(catnip.PerTick.Positive != true || catnip.PerTick.Delta < 8)
            {
                Building = Buildings.CatnipField;
                return;
            }

            Resource kittens = Helper.getResource("kittens");
            if(kittens == null || kittens.MaxAmount < 4)
            {
                Building = Buildings.Hut;
                return;
            }

            if(Buildings.Library.Available && Buildings.Library.Count == 0)
            {
                Building = Buildings.Library;
                return;
            }
        }
    }
}
