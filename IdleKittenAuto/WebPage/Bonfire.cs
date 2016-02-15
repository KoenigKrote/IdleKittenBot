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
    public class Bonfire : Program
    {
        public IWebDriver _driver;
        private Actions _actions;
        private string SaveData = string.Empty;
        private int counter = 0;

        #region Controls
        [FindsBy(How = How.ClassName, Using = @"activeTab")]
        private IWebElement activeTab;

        [FindsBy(How = How.XPath, Using = @"//*[@id='resContainer']/table")]
        private IWebElement resourceTable;

        [FindsBy(How = How.XPath, Using = @"//*[@id='gameContainerId']/div[2]/div/div/table[12]")]
        private IWebElement btnTable;

        //Building controls
        private IWebElement btnGatherCatnip;
        private IWebElement btnBuildCatnipField;
        private IWebElement btnRefineCatnip;
        private IWebElement btnBuildHut;
        private IWebElement btnBuildPasture;
        private IWebElement btnBuildLibrary;

        //Other shit
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
            //loadData();
            //PageFactory.InitElements(_driver, this);
            buildBtnTable();
            while (true)
            {
                for (int i = 0; i < 20; i++)
                {
                    Helper.updateResources(_driver);
                    Objective.determineObjective();
                    gatherCatnip();
                    ObjectiveBuildings();
                }

                saveData();
            }
        }

        //TODO: Does not work correctly after first hut
        private void ObjectiveBuildings()
        {
            switch(Objective.Building.Name)
            {
                case "Catnip Field":
                    buildCatnipField();
                    break;
                case "Hut":
                    buildHut();
                    break;
            }
        }

        private void ObjectiveJobs()
        {
            btnVillagePage.Click();
            
        }

        private void collectCatnip()
        {
            while (Objective.Building.Count == Buildings.CatnipField.Count)
            {
                gatherCatnip();
                buildCatnipField();
            }
        }

        //TODO: Undoubtedly there's a better way to do this
        private void buildBtnTable()
        {
            IList<IWebElement> btnList = btnTable.FindElements(By.XPath(@".//tr/td[*]/div[*]"));
            btnGatherCatnip = btnList.FirstOrDefault(b => b.Text.Contains("Gather"));
            btnRefineCatnip = btnList.FirstOrDefault(b => b.Text.Contains("Refine catnip"));
            btnBuildCatnipField = btnList.FirstOrDefault(b => b.Text.Contains("Catnip field"));
            btnBuildHut = btnList.FirstOrDefault(b => b.Text.Contains("Hut"));
            btnBuildPasture = btnList.FirstOrDefault(b => b.Text.Contains("Pasture"));
            btnBuildLibrary = btnList.FirstOrDefault(b => b.Text.Contains("Library"));
            PageFactory.InitElements(_driver, this);
        }

        private void buildStructures()
        {
            buildCatnipField();
            buildHut();
            buildPasture();
            buildLibrary();
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
                try
                {
                    btnGatherCatnip.Click();
                }
                catch
                {
                    buildBtnTable();
                }
                Thread.Sleep(125); //Short pause to allow the browser to catch up
            }
        }

        private void refineCatnip()
        {
            Resource catnip = Helper.getResource("catnip");
            if (catnip.Amount > 100)
            {
                Resource wood = Helper.getResource("wood");
                if (wood == null || catnip.PerTick.Positive == true && catnip.PerTick.Delta > 8)
                    btnRefineCatnip.Click();
            }
        }

        //Oh god please forgive me
        private void buildCatnipField()
        {
            if (btnBuildCatnipField == null)
                return;
            else
                Buildings.CatnipField.Available = true;

            Resource catnip = Helper.getResource("catnip");
            Resource kittens = Helper.getResource("kittens");

            var oldCount = Buildings.CatnipField.Count;
            try
            {
                Buildings.CatnipField.Count = Helper.StripNonNum(btnBuildCatnipField.Text);
            }
            catch
            {
                buildBtnTable();
            }

            if (Buildings.CatnipField.Count == 0 &&
                Buildings.CatnipField.Requirements["catnip"] < catnip.Amount)
            {
                btnBuildCatnipField.Click();
                return;
            }

            //TODO: What the fuck
            if (Helper.TimeToEven(Buildings.CatnipField.Requirements["catnip"],
                Buildings.CatnipField.Produces.Resources["catnip"].PerTick.Delta) < 1800 &&
                Buildings.CatnipField.Requirements["catnip"] < catnip.Amount)
            {
                //if(catnip.PerTick.Positive == false || kittens == null ||
                //    (kittens.Amount * 8) < CatnipField.Count)
                //{
                try
                {
                    btnBuildCatnipField.Click();
                    Buildings.CatnipField.Requirements["catnip"] *= Buildings.CatnipField.Ratio;
                    return;
                }
                catch
                {
                    buildBtnTable();
                }

                //}
            }
        }

        private void buildHut()
        {
            if (btnBuildHut == null)
            {
                refineCatnip();
                return;
            }
            else
                Buildings.Hut.Available = true;
            Resource wood = Helper.getResource("wood");
            if (wood == null)
            {
                refineCatnip();
                return;
            }

            Resource catnip = Helper.getResource("catnip");
            Resource kittens = Helper.getResource("kittens");

            try
            {
                Buildings.Hut.Count = Helper.StripNonNum(btnBuildHut.Text);
            }
            catch
            {
                buildBtnTable();
            }

            //If we haven't built any huts, build our first and return.
            if (Buildings.Hut.Count == 0 && 
                Buildings.Hut.Requirements["wood"] <= wood.Amount)
            {
                btnBuildHut.Click();
                Buildings.Hut.Requirements["wood"] *= Buildings.Hut.Ratio;
                return;
            }
            else
            {
                refineCatnip();
            }

            //Make sure we have positive catnip income before building anything.
            if (catnip.PerTick.Positive == true && catnip.PerTick.Delta >= 10 &&
                Buildings.Hut.Requirements["wood"] <= wood.Amount)
            {
                if (kittens == null || kittens.Amount == kittens.MaxAmount)
                {
                    btnBuildHut.Click();
                    Buildings.Hut.Requirements["wood"] *= Buildings.Hut.Ratio;
                    return;
                }
            }
            else if (Objective.Building == Buildings.Hut && 
                Buildings.Hut.Requirements["wood"] <= wood.Amount)
            {
                buildCatnipField();
            }
            else if(Objective.Building == Buildings.Hut &&
                Buildings.Hut.Requirements["wood"] > wood.Amount)
            {
                refineCatnip();
            }
        }

        private void buildPasture()
        {
            if (btnBuildPasture == null)
                return;
            else
                Buildings.Pasture.Available = true;

            Resource wood = Helper.getResource("wood");
            if (wood == null) return;
            Resource kittens = Helper.getResource("kittens");
            if (kittens == null) return;
            Resource catnip = Helper.getResource("catnip");

            if (catnip.PerTick.Positive == true && catnip.PerTick.Delta >= 18 &&
                Buildings.Pasture.Count < 50 && 
                Buildings.Pasture.Requirements["catnip"] < catnip.Amount &&
                Buildings.Pasture.Requirements["wood"] < wood.Amount)
            {
                try
                {
                    btnBuildPasture.Click();
                    Buildings.Pasture.Requirements["wood"] *= Buildings.Pasture.Ratio;
                    Buildings.Pasture.Requirements["catnip"] *= Buildings.Pasture.Ratio;
                }
                catch
                {
                    buildBtnTable();
                }
            }
        }

        private void buildLibrary()
        {
            if (btnBuildLibrary == null)
                return;
            else
                Buildings.Library.Available = true;

            Resource wood = Helper.getResource("wood");

            try
            {
                Buildings.Library.Count = Helper.StripNonNum(btnBuildLibrary.Text);
            }
            catch
            {
                buildBtnTable();
            }

            if (Buildings.Library.Requirements["wood"] <= wood.Amount)
            {
                btnBuildLibrary.Click();
                Buildings.Library.Requirements["wood"] *= Buildings.Library.Ratio;
                return;
            }
        }


    }


}
