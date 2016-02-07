using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.PageObjects;
using OpenQA.Selenium.Interactions;

namespace IdleKittenAuto.WebPage
{
    public class Bonfire
    {
        public IWebDriver _driver;
        public List<Resource> _resourceList = new List<Resource>();
        private BuildingList _buildings = new BuildingList();
        private Regex rateRegex = new Regex(@"([0-9]*\.[0-9]*)|([0-9]+)");
        private Actions _actions;
        private string SaveData = string.Empty;

        #region Controls
        [FindsBy(How = How.ClassName, Using = @"activeTab")]
        private IWebElement activeTab;

        [FindsBy(How = How.XPath, Using = @"//*[@id='resContainer']/table")]
        private IWebElement resourceTable;

        [FindsBy(How = How.XPath, Using = @"//*[@id='gameContainerId']/div[2]/div/div/table[12]/tr/td[1]/div[1]")]
        private IWebElement btnGetCatnip;

        [FindsBy(How = How.XPath, Using = @"//*[@id='gameContainerId']/div[2]/div/div/table[12]/tr/td[1]/div[2]")]
        private IWebElement btnBuildCatnipField;

        [FindsBy(How = How.XPath, Using = @"//*[@id='gameContainerId']/div[2]/div/div/table[12]/tr/td[2]/div[1]")]
        private IWebElement btnRefineCatnip;

        [FindsBy(How = How.XPath, Using = @"//*[@id='gameContainerId']/div[2]/div/div/table[12]/tr/td[2]/div[2]")]
        private IWebElement btnBuildHut;

        [FindsBy(How = How.XPath, Using = @"//*[@id='tooltip']/div/div[2]")]
        private IWebElement toolTip;

        [FindsBy(How = How.XPath, Using = @"//*[@id='headerLinks']/a[2]")]
        private IWebElement btnOptions;

        [FindsBy(How = How.XPath, Using = @"//*[@id='optionsDiv']/a[1]")]
        private IWebElement btnOptionsClose;

        [FindsBy(How = How.XPath, Using = @"//*[@id='optionsDiv']/input[7]")]
        private IWebElement btnExport;

        [FindsBy(How = How.XPath, Using = @"//*[@id='optionsDiv']/input[8]")]
        private IWebElement btnImport;

        [FindsBy(How = How.XPath, Using = @"//*[@id='exportData']")]
        private IWebElement txtDataSave;

        [FindsBy(How = How.XPath, Using = @"//*[@id='importData']")]
        private IWebElement txtDataLoad;

        [FindsBy(How = How.XPath, Using = @"//*[@id='exportDiv']/input")]
        private IWebElement btnExportClose;

        [FindsBy(How = How.XPath, Using = @"//*[@id='importDiv']/input[1]")]
        private IWebElement btnImportOk;
        #endregion

        public void MainLoop()
        {
            _driver = new FirefoxDriver();
            _actions = new Actions(_driver);
            _driver.Navigate().GoToUrl(@"http://bloodrizer.ru/games/kittens/");
            PageFactory.InitElements(_driver, this);
            //loadData();
            while (true)
            {
                getResourceData();
                gatherCatnip();
                refineCatnip();
                buildStructures();

            }
        }

        private void buildStructures()
        {
            buildCatnipField();
            buildHut();
        }

        private void buildHut()
        {
            Resource catnip = _resourceList.Where(r => r.Name.ToLower() == "catnip").First();
            Resource wood = _resourceList.Where(r => r.Name.ToLower() == "wood").FirstOrDefault();
            Resource kittens = _resourceList.Where(r => r.Name.ToLower() == "kittens").FirstOrDefault();
            _buildings.Hut.Requirements = _buildings.Hut.BaseRequirements;
            if (wood == null) return;

            if (_buildings.Hut.Count == 0 && _buildings.Hut.Requirements["wood"] <= wood.Amount)
            {
                btnBuildHut.Click();
                return;
            }


            for (int i = 0; i < _buildings.Hut.Count; i++)
                _buildings.Hut.Requirements["wood"] *= _buildings.Hut.Ratio;

            if (catnip.PerTick.Positive == true && catnip.PerTick.Delta >= 18 &&
                _buildings.Hut.Requirements["wood"] <= wood.Amount)
            {
                if (kittens == null)
                {
                    btnBuildHut.Click();
                    return;
                }
                if (kittens.Amount == kittens.MaxAmount)
                {
                    btnBuildHut.Click();
                    return;
                }
            }

        }

        //Opens options, saves export data string to a text file.
        private void saveData()
        {
            btnOptions.Click();
            btnExport.Click();
            Thread.Sleep(500);
            SaveData = txtDataSave.GetAttribute("value");
            File.WriteAllText(@".\Save.txt", SaveData);
            Thread.Sleep(500);
            btnExportClose.Click();
            btnOptionsClose.Click();
        }

        //Opens options, imports export data string from text file
        private void loadData()
        {
            SaveData = File.ReadAllText(@".\Save.txt");
            btnOptions.Click();
            btnImport.Click();
            txtDataLoad.SendKeys(SaveData);
            Thread.Sleep(500);
            btnImportOk.Click();
            _driver.SwitchTo().Alert().Accept();
        }



        //Loads resource data into a list for logic checking
        private void getResourceData()
        {
            _resourceList.Clear();
            IList<IWebElement> resourceRows = resourceTable.FindElements(By.XPath(@".//tr"));
            for (int i = 0; i < resourceRows.Count; i++)
            {
                if (!string.IsNullOrWhiteSpace(resourceRows[i].Text))
                {
                    var row = resourceRows[i].FindElements(By.XPath(@".//td"));
                    _resourceList.Add(new Resource
                    {
                        Name = StripNonChar(row[0].Text),
                        Amount = StripNonNum(row[1].Text),
                        MaxAmount = StripNonNum(row[2].Text),
                        PerTick = ParseRate(row[3].Text)
                    });
                }
            }
            if (_resourceList.Count == 0)
            {
                btnGetCatnip.Click();
                getResourceData();
            }
        }

        //Prototype logic for manually gathering catnip
        private void gatherCatnip()
        {
            for (int i = 0; i < 10; i++)
            {
                btnGetCatnip.Click();
                Thread.Sleep(125); //Short pause to allow the browser to catch up
            }
        }

        //Oh god please forgive me
        private void buildCatnipField()
        {
            Resource catnip = _resourceList.Where(r => r.Name.ToLower() == "catnip").First();
            Resource kittens = _resourceList.Where(r => r.Name.ToLower() == "kittens").FirstOrDefault();
            _buildings.CatnipField.Count = StripNonNum(btnBuildCatnipField.Text);
            _buildings.CatnipField.Requirements["catnip"] = _buildings.CatnipField.BaseRequirements["catnip"];

            if (_buildings.CatnipField.Count == 0 &&
                _buildings.CatnipField.Requirements["catnip"] < catnip.Amount)
            {
                btnBuildCatnipField.Click();
                return;
            }

            for (int i = 0; i < _buildings.CatnipField.Count; i++)
                _buildings.CatnipField.Requirements["catnip"] *= _buildings.CatnipField.Ratio;

            //TODO: What the fuck
            if (TimeToEven(_buildings.CatnipField.Requirements["catnip"],
                _buildings.CatnipField.Produces[0].PerSecond.Delta) < 1800 &&
                _buildings.CatnipField.Requirements["catnip"] < catnip.Amount)
            {
                if (kittens == null)
                {
                    btnBuildCatnipField.Click();
                    return;
                }

                if((kittens.Amount * 2) < _buildings.CatnipField.Count)
                    btnBuildCatnipField.Click();
            }
        }

        private void refineCatnip()
        {
            Resource catnip = _resourceList.Where(r => r.Name.ToLower() == "catnip").First();
            if (catnip.Amount > 100)
            {
                Resource wood = _resourceList.Where(r => r.Name.ToLower() == "wood").FirstOrDefault();
                if (wood == null)
                    btnRefineCatnip.Click();

                if (catnip.PerTick.Positive == true && catnip.PerTick.Delta > 10)
                    btnRefineCatnip.Click();
            }
        }

        //Strips non-alpha characters from a given string
        private string StripNonChar(string input)
        {
            input = new string(input.Where(c => char.IsLetter(c)).ToArray());
            return input;
        }

        //Strips non-numerical characters from a given string
        private double StripNonNum(string input)
        {
            double number;
            input = new string(input.Where(c => char.IsDigit(c) || c == '.').ToArray());
            double.TryParse(input, out number);
            return number;
        }

        //Parses the per-second rate for a resource and converts it into a Rate object
        private Rate ParseRate(string input)
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
        private int TimeToZero(Resource input)
        {
            return (int)Math.Ceiling((input.Amount / input.PerTick.Delta));
        }

        //Gets the remaining amount to capacity, divides it by current positive delta.
        private int TimeToMax(Resource input)
        {
            return (int)Math.Ceiling(((input.MaxAmount - input.Amount) / input.PerTick.Delta));
        }

        private int TimeToEven(double amount, double delta)
        {
            return (int)Math.Floor(amount / delta);
        }
    }

    public class Resource
    {
        public string Name { get; set; } //Name of resource
        public double Amount { get; set; } //Current amount of resource
        public double MaxAmount { get; set; } //Maximum storage amount for resource
        public double MinAmount { get; set; } //Smallest amount logic will allow before acting
        public Rate PerTick { get; set; } //Gives delta change and whether it is increase/descrease
        public int SecToZero { get; set; } //Minutes to Zero of resource
        public int SecToMax { get; set; } //Minutes to Maximum of resource
    }

    public class Rate
    {
        public bool? Positive { get; set; } //Nullable bool for positive change, negative change, and no change
        public double Delta { get; set; } //Actual delta of change
    }
}
