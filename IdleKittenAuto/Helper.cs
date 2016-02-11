using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.PageObjects;
using OpenQA.Selenium.Interactions;
using IdleKittenAuto.WebPage;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace IdleKittenAuto
{
    public static class Helper
    {
        private static Regex rateRegex = new Regex(@"([0-9]*\.[0-9]*)|([0-9]+)");
        public static List<Resource> _resources = new List<Resource>();
        public static int prevKittenCount = 0;


        //Cheap D.R.Y. fix
        public static void updateResources(IWebDriver _driver)
        {
            ResourceList resources = new ResourceList(_driver, _resources);
            _resources = resources.getResourceData();
            resources.checkForJobAssignment();
        }
        
        //Strips non-alpha characters from a given string
        public static string StripNonChar(string input)
        {
            input = new string(input.Where(c => char.IsLetter(c)).ToArray());
            return input;
        }

        //Strips non-numerical characters from a given string
        public static double StripNonNum(string input)
        {
            double number;
            input = new string(input.Where(c => char.IsDigit(c) || c == '.').ToArray());
            double.TryParse(input, out number);
            return number;
        }

        //Parses the per-second rate for a resource and converts it into a Rate object
        public static Rate ParseRate(string input)
        {
            Rate rate = new Rate();
            if (input.Contains('+'))
                rate.Positive = true;
            else if (input.Contains('-'))
                rate.Positive = false;
            else
                rate.Positive = null;
            if (rate.Positive != null)
                rate.Delta = double.Parse(rateRegex.Match(input).ToString());
            return rate;
        }

        //Divides current amount by negative delta
        public static int TimeToZero(Resource input)
        {
            return (int)Math.Ceiling((input.Amount / input.PerTick.Delta));
        }

        //Gets the remaining amount to capacity, divides it by current positive delta.
        public static int TimeToMax(Resource input)
        {
            return (int)Math.Ceiling(((input.MaxAmount - input.Amount) / input.PerTick.Delta));
        }

        public static int TimeToEven(double amount, double delta)
        {
            return (int)Math.Floor(amount / delta);
        }

        public static Resource getResource(string resourceName)
        {
            Resource resource = _resources.FirstOrDefault(r => r.Name.ToLower() == resourceName.ToLower());
            return resource;
        }
    }
}
