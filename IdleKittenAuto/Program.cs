using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdleKittenAuto.WebPage;

namespace IdleKittenAuto
{
    public class Program
    {
        static void Main(string[] args)
        {
            Bonfire _bonfire = new Bonfire();
            _bonfire.MainLoop();
        }
    }
}
