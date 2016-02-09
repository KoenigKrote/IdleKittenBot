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
            Resource kittens = _resourceList.Where(r => r.Name.ToLower() == "kittens").FirstOrDefault();
            if (kittens != null)
            {
                Resource catnip = _resourceList.Where(r => r.Name.ToLower() == "catnip").FirstOrDefault();
                if (kittens.Amount > Helper.prevKittenCount)
                {
                    Helper.prevKittenCount = (int)Math.Floor(kittens.Amount);
                    gotoVillage(_driver, _resourceList);
                    btnBonfirePage.Click();
                }
                else if (catnip.PerTick.Positive == false || catnip.PerTick.Delta >= 11 &&
                    JobDictionary.Dictionary["farmer"].Count > 0)
                {
                    gotoVillage(_driver, _resourceList);
                    btnBonfirePage.Click();
                }
            }
            return _resourceList;
        }

        private Village gotoVillage(IWebDriver _driver, List<Resource> _resourceList)
        {
            btnVillagePage.Click();
            return new Village(_driver, _resourceList);
        }
    }
}
