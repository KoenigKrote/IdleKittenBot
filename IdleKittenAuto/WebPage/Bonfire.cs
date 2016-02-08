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
        private Dictionary<string, Building> _buildings = Buildings.Dictionary;
        
        private Actions _actions;
        private string SaveData = string.Empty;
        private int prevKittenCount = 0;

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

        [FindsBy(How = How.XPath, Using = @"//*[@id='gameContainerId']/div[1]/a[2]")]
        private IWebElement btnVillagePage;

        [FindsBy(How = How.XPath, Using = @"//*[@id='gameContainerId']/div[1]/a[1]")]
        private IWebElement btnBonfirePage;
        #endregion

        public void MainLoop()
        {
            _driver = new FirefoxDriver();
            _actions = new Actions(_driver);
            _driver.Navigate().GoToUrl(@"http://bloodrizer.ru/games/kittens/");
            PageFactory.InitElements(_driver, this);
            loadData();
            PageFactory.InitElements(_driver, this);
            while (true)
            {
                _resourceList = Helper.updateResources(_driver, _resourceList);
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

        private void gatherCatnip()
        {
            for (int i = 0; i < 10; i++)
            {
                btnGetCatnip.Click();
                Thread.Sleep(125); //Short pause to allow the browser to catch up
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

        //Oh god please forgive me
        private void buildCatnipField()
        {
            Resource catnip = _resourceList.Where(r => r.Name.ToLower() == "catnip").First();
            Resource kittens = _resourceList.Where(r => r.Name.ToLower() == "kittens").FirstOrDefault();
            var oldCount = _buildings["catnipfield"].Count;
            _buildings["catnipfield"].Count = Helper.StripNonNum(btnBuildCatnipField.Text);
            
            if (_buildings["catnipfield"].Count == 0 &&
                _buildings["catnipfield"].Requirements["catnip"] < catnip.Amount)
            {
                btnBuildCatnipField.Click();
                return;
            }


            for (int i = 0; i < (_buildings["catnipfield"].Count - oldCount); i++)
                _buildings["catnipfield"].Requirements["catnip"] *= _buildings["catnipfield"].Ratio;

            //TODO: What the fuck
            if (Helper.TimeToEven(_buildings["catnipfield"].Requirements["catnip"],
                _buildings["catnipfield"].Produces[0].PerSecond.Delta) < 1800 &&
                _buildings["catnipfield"].Requirements["catnip"] < catnip.Amount)
            {
                if (kittens == null)
                {
                    btnBuildCatnipField.Click();

                    return;
                }

                if((kittens.Amount * 2) < _buildings["catnipfield"].Count)
                    btnBuildCatnipField.Click();
            }
        }

        private void buildHut()
        {
            Resource catnip = _resourceList.Where(r => r.Name.ToLower() == "catnip").First();
            Resource wood = _resourceList.Where(r => r.Name.ToLower() == "wood").FirstOrDefault();
            Resource kittens = _resourceList.Where(r => r.Name.ToLower() == "kittens").FirstOrDefault();

            var oldCount = _buildings["catnipfield"].Count;
            _buildings["catnipfield"].Count = Helper.StripNonNum(btnBuildHut.Text);
            if (wood == null) return;

            if (_buildings["hut"].Count == 0 && _buildings["hut"].Requirements["wood"] <= wood.Amount)
            {
                btnBuildHut.Click();
                return;
            }


            for (int i = 0; i < (_buildings["hut"].Count - oldCount); i++)
                _buildings["hut"].Requirements["wood"] *= _buildings["hut"].Ratio;

            if (catnip.PerTick.Positive == true && catnip.PerTick.Delta >= 18 &&
                _buildings["hut"].Requirements["wood"] <= wood.Amount)
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
