using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.PageObjects;
using OpenQA.Selenium.Interactions;
using IdleKittenAuto.WebPage;

namespace IdleKittenAuto.WebPage
{
    public class ResourceList
    {
        private IWebDriver _driver;
        private List<Resource> _resourceList;

        public ResourceList(IWebDriver _driver, List<Resource> _resourceList)
        {
            this._driver = _driver;
            this._resourceList = _resourceList;
            PageFactory.InitElements(_driver, this);
        }

        [FindsBy(How = How.XPath, Using = @"//*[@id='resContainer']/table")]
        private IWebElement resourceTable;

        [FindsBy(How = How.XPath, Using = @"//*[@id='gameContainerId']/div[1]/a[1]")]
        private IWebElement btnBonfirePage;

        [FindsBy(How = How.XPath, Using = @"//*[@id='gameContainerId']/div[1]/a[2]")]
        private IWebElement btnVillagePage;

        [FindsBy(How = How.XPath, Using = @"//*[@id='gameContainerId']/div[2]/div/div/table[12]/tr/td[1]/div[1]")]
        private IWebElement btnGetCatnip;

        [FindsBy(How = How.ClassName, Using = @"activeTab")]
        private IWebElement activeTab;

        public List<Resource> getResourceData()
        {
            PageFactory.InitElements(_driver, this);
            _resourceList.Clear();
            IList<IWebElement> resourceRows = resourceTable.FindElements(By.XPath(@".//tr"));
            for (int i = 0; i < resourceRows.Count; i++)
            {
                if (!string.IsNullOrWhiteSpace(resourceRows[i].Text))
                {
                    var row = resourceRows[i].FindElements(By.XPath(@".//td"));
                    _resourceList.Add(new Resource
                    {
                        Name = Helper.StripNonChar(row[0].Text),
                        Amount = Helper.StripNonNum(row[1].Text),
                        MaxAmount = Helper.StripNonNum(row[2].Text),
                        PerTick = Helper.ParseRate(row[3].Text)
                    });
                }
            }
            if (_resourceList.Count == 0)
            {
                btnGetCatnip.Click();
                getResourceData();
            }

            return _resourceList;
        }

        public void checkForJobAssignment()
        {
            Resource kittens = Helper.getResource("kittens");
            if (kittens != null)
            {
                Resource catnip = Helper.getResource("catnip");
                if (kittens.Amount > Helper.prevKittenCount)
                {
                    Helper.prevKittenCount = (int)Math.Floor(kittens.Amount);
                    gotoVillage(_driver);
                    btnBonfirePage.Click();
                }
                else if (catnip.PerTick.Positive == false || catnip.PerTick.Delta >= 11 &&
                    JobDictionary.Dictionary["farmer"].Count > 0)
                {
                    gotoVillage(_driver);
                    btnBonfirePage.Click();
                }
            }
        }


        private Village gotoVillage(IWebDriver _driver)
        {
            btnVillagePage.Click();
            return new Village(_driver);
        }
    }

    public static class Resources
    {
        public static Resource Catnip { get; set; }
        public static Resource Wood { get; set; }
        public static Resource Minerals { get; set; }
        public static Resource Coal { get; set; }
        public static Resource Iron { get; set; }
        public static Resource Gold { get; set; }
        public static Resource Oil { get; set; }
        public static Resource Titanium { get; set; }
        public static Resource Uranium { get; set; }
        public static Resource Unobtainium { get; set; }
        public static Resource Antimatter { get; set; }
        public static Resource Catpower { get; set; }
        public static Resource Science { get; set; }
        public static Resource Culture { get; set; }
        public static Resource Faith { get; set; }
        public static Resource Energy { get; set; }
        public static Resource Kittens { get; set; }
        public static Resource Zebras { get; set; }
        public static Resource Starchart { get; set; }
        public static Resource TimeCrystal { get; set; }
        public static Resource Sorrow { get; set; }
        public static Resource Relic { get; set; }
    }

    public class Resource
    {
        public string Name { get; set; } //Name of resource
        public double Amount { get; set; } //Current amount of resource
        public double MaxAmount { get; set; } //Maximum storage amount for resource
        public double MinAmount { get; set; } //Smallest amount logic will allow before acting
        public Rate PerTick { get; set; } //Gives delta change and whether it is increase/descrease
        public Rate Demand { get; set; } //Demand listed in game for the resource
        public int SecToZero { get; set; } //Minutes to Zero of resource
        public int SecToMax { get; set; } //Minutes to Maximum of resource
    }

    public class Rate
    {
        public string Name { get; set; } //Optional string for percentage changes on buildings
        public bool? Positive { get; set; } //Nullable bool for positive change, negative change, and no change
        public double Delta { get; set; } //Actual delta of change
    }
}
