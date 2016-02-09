using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.PageObjects;
using OpenQA.Selenium.Interactions;
using System.Text.RegularExpressions;

namespace IdleKittenAuto.WebPage
{
    public class Village : Program
    {
        IWebDriver _driver;
        List<Resource> _resourceList;
        public Village(IWebDriver _driver, List<Resource> _resourceList)
        {
            this._driver = _driver;
            this._resourceList = _resourceList;
            PageFactory.InitElements(_driver, this);
            getJobData();
            assignJobs();
        }

        public List<Job> _jobList = new List<Job>();
        private Regex jobRegex = new Regex(@"([A-Z])[^(]*");
        private Regex freeRegex = new Regex(@"([0-9]+.\/.[0-9]+)");

        [FindsBy(How = How.XPath, Using = @"//*[@id='gameContainerId']/div[1]/a[1]")]
        private IWebElement btnBonfirePage;

        [FindsBy(How = How.XPath, Using = @"//*[@id='gameContainerId']/div[2]/div[1]/div[3]")]
        private IWebElement jobTable;

        [FindsBy(How = How.XPath, Using = @"//*[@id='gameContainerId']/div[2]/div[1]/div[3]/div[1]/div/div[2]/a")]
        private IWebElement btnUpWoodcutter;

        [FindsBy(How = How.XPath, Using = @"//*[@id='gameContainerId']/div[2]/div[1]/div[3]/div[1]/div/div[1]/a")]
        private IWebElement btnDownWoodcutter;

        [FindsBy(How = How.XPath, Using = @"//*[@id='gameContainerId']/div[2]/div[1]/div[3]/div[2]/div/div[2]/a")]
        private IWebElement btnUpFarmer;

        [FindsBy(How = How.XPath, Using = @"//*[@id='gameContainerId']/div[2]/div[1]/div[3]/div[2]/div/div[1]/a")]
        private IWebElement btnDownFarmer;

        [FindsBy(How = How.XPath, Using = @"//*[@id='gameContainerId']/div[2]/div[1]/div[3]/div[3]/div/div[2]/a")]
        private IWebElement btnUpScholar;

        [FindsBy(How = How.XPath, Using = @"//*[@id='gameContainerId']/div[2]/div[1]/div[3]/div[3]/div/div[1]/a")]
        private IWebElement btnDownScholar;

        [FindsBy(How = How.XPath, Using = @"//*[@id='gameContainerId']/div[2]/div[1]/div[3]/div[4]/div/div[2]/a")]
        private IWebElement btnUpHunter;

        [FindsBy(How = How.XPath, Using = @"//*[@id='gameContainerId']/div[2]/div[1]/div[3]/div[4]/div/div[1]/a")]
        private IWebElement btnDownHunter;

        [FindsBy(How = How.XPath, Using = @"//*[@id='gameContainerId']/div[2]/div[1]/div[3]/div[5]/div/div[2]/a")]
        private IWebElement btnUpMiner;

        [FindsBy(How = How.XPath, Using = @"//*[@id='gameContainerId']/div[2]/div[1]/div[3]/div[5]/div/div[1]/a")]
        private IWebElement btnDownMiner;

        //[FindsBy(How = How.XPath, Using = @"//*[@id='gameContainerId']/div[1]/a[1]")]
        //[FindsBy(How = How.XPath, Using = @"//*[@id='gameContainerId']/div[1]/a[1]")]
        //[FindsBy(How = How.XPath, Using = @"//*[@id='gameContainerId']/div[1]/a[1]")]
        //[FindsBy(How = How.XPath, Using = @"//*[@id='gameContainerId']/div[1]/a[1]")]
        //[FindsBy(How = How.XPath, Using = @"//*[@id='gameContainerId']/div[1]/a[1]")]


        public void getJobData()
        {
            _jobList.Clear();
            var unemployed = jobTable.FindElement(By.XPath(@".//table/tr[2]/td"));
            _jobList.Add(JobDictionary.Dictionary["unemployed"]);
            _jobList[0].Count = FreeKittens(unemployed.Text);
            IList<IWebElement> jobRows = jobTable.FindElements(By.XPath(".//div[*]/div/span"));
            for (int i = 0; i < jobRows.Count; i++)
            {
                if(!string.IsNullOrWhiteSpace(jobRows[i].Text) && jobRows[i].Text != "Clear")
                {
                    _jobList.Add(JobDictionary.Dictionary[jobRegex.Match(jobRows[i].Text).ToString().Trim().ToLower()]);
                    _jobList[i+1].Count = StripNonNum(jobRows[i].Text);
                }
            }
        }

        public void assignJobs()
        {
            assignUnemployed();
            Resource catnip = _resourceList.Where(r => r.Name.ToLower() == "catnip").First();
            int i = 0;
            //TODO: Refactor pulling resources, this shit is ugly.
            while(catnip.PerTick.Positive == false && i < _jobList.Count)
            {
                if(_jobList[i].Count > 0)
                {
                    upFarmers(i);
                }
                i++;
            }


            while(catnip.PerTick.Positive == true && catnip.PerTick.Delta >= 9 &&
                JobDictionary.Dictionary["farmer"].Count > 0)
            {
                if(Library.Count > 0)
                {
                    btnDownFarmer.Click();
                    btnUpScholar.Click();
                }
            }
        }

        private void upFarmers(int i)
        {
            switch (_jobList[i].Title.ToLower())
            {
                case "hunter":
                    btnDownHunter.Click();
                    btnUpFarmer.Click();
                    _resourceList = Helper.updateResources(_driver, _resourceList);
                    break;
                case "woodcutter":
                    btnDownWoodcutter.Click();
                    btnUpFarmer.Click();
                    _resourceList = Helper.updateResources(_driver, _resourceList);
                    break;
                case "miner":
                    btnDownMiner.Click();
                    btnUpFarmer.Click();
                    _resourceList = Helper.updateResources(_driver, _resourceList);
                    break;
                case "scholar":
                    btnDownScholar.Click();
                    btnUpFarmer.Click();
                    _resourceList = Helper.updateResources(_driver, _resourceList);
                    break;
            }
        }

        private void jobUp(int i)
        {
            switch (_jobList[i].Title.ToLower())
            {
                case "hunter":
                    btnUpHunter.Click();
                    _resourceList = Helper.updateResources(_driver, _resourceList);
                    break;
                case "woodcutter":
                    btnUpWoodcutter.Click();
                    _resourceList = Helper.updateResources(_driver, _resourceList);
                    break;
                case "miner":
                    btnUpMiner.Click();
                    _resourceList = Helper.updateResources(_driver, _resourceList);
                    break;
                case "scholar":
                    btnUpScholar.Click();
                    _resourceList = Helper.updateResources(_driver, _resourceList);
                    break;
            }
            }

        private void assignUnemployed()
        {
            for (int i = 0; i < _jobList[0].Count; i++)
            {
                Resource catnip = _resourceList.Where(r => r.Name.ToLower() == "catnip").First();
                if (catnip.PerTick.Positive == false)
                {
                    btnUpFarmer.Click();
                    _resourceList = Helper.updateResources(_driver, _resourceList);
                    continue;
                }
                if(_jobList[i].Count < 1)
                {
                    jobUp(i);
                }
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

        private int FreeKittens (string input)
        {
            string FreeKitten = freeRegex.Match(input).ToString().Split('/').First();
            int Free = int.Parse(FreeKitten);
            return Free;
        }
    }
}
