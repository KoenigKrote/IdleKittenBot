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
using System.Threading;

namespace IdleKittenAuto.WebPage
{
    public class Village : Program
    {
        IWebDriver _driver;
        public Village(IWebDriver _driver)
        {
            this._driver = _driver;
            PageFactory.InitElements(_driver, this);
            getJobData();
            AssignJobs();
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

        //Parse the HTML table holding all the job buttons and job count
        public void getJobData()
        {
            var unemployed = jobTable.FindElement(By.XPath(@".//table/tr[2]/td"));
            Jobs.Job["unemployed"].Count = FreeKittens(unemployed.Text);
            IList<IWebElement> jobRows = jobTable.FindElements(By.XPath(".//div[*]/div/span"));
            for (int i = 0; i < jobRows.Count; i++)
            {
                if (!string.IsNullOrWhiteSpace(jobRows[i].Text) && jobRows[i].Text != "Clear")
                {
                    //TODO: Rewrite this so it's not so damn ugly.
                    string job = jobRegex.Match(jobRows[i].Text).ToString().Trim().ToLower();
                    Jobs.Job[job].Count = Helper.StripNonNum(jobRows[i].Text);
                    Jobs.Job[job].Available = true;
                }
            }
        }

        //public void assignJobs()
        //{
        //    AssignJobs((int)Jobs.Job["unemployed"].Count);
        //    Resource catnip = Helper.getResource("catnip");
        //    int i = 0;



        //    //TODO: Rework this, I don't think the logic is sound.
        //    while (catnip.PerTick.Positive == false && i < _jobList.Count)
        //    {
        //        if (_jobList[i].Count > 0)
        //        {
        //            upFarmers(i);
        //        }
        //        i++;
        //        catnip = Helper.getResource("catnip");
        //    }


        //    while (catnip.PerTick.Positive == true && catnip.PerTick.Delta >= 9 &&
        //        Jobs.Job["farmer"].Count > 0)
        //    {
        //        if (Buildings.Library.Count > 0)
        //        {
        //            btnDownFarmer.Click();
        //            btnUpScholar.Click();
        //        }
        //        catnip = Helper.getResource("catnip");
        //    }
        //}



        private void AssignJobs()
        {
            Resource kittens = Helper.getResource("kittens");
            for (int i = 0; i < kittens.Amount; i++)
            {
                //Always check to make sure we have positive catnip before chasing objectives
                Resource catnip = Helper.getResource("catnip");
                if ((catnip.PerTick.Positive == false || catnip.PerTick.Delta < 8) &&
                    Jobs.Job["farmer"].Available)
                {
                    btnUpFarmer.Click();
                    Thread.Sleep(250);
                    Helper.updateResources(_driver);
                    continue;
                }
                else
                {
                    foreach (var item in Objective.ResourceGoal)
                    {
                        int toAssign = Helper.kittensToAssign(item.Value);
                        for (int j = 0; j < toAssign; j++)
                        {
                            jobUp(item.Key);
                        }
                    }
                }
            }
        }

        private int FreeKittens(string input)
        {
            string FreeKitten = freeRegex.Match(input).ToString().Split('/').First();
            int Free = int.Parse(FreeKitten);
            return Free;
        }

        //Lazy switch to move other jobs to farming
        private void upFarmers(int i)
        {
            switch (_jobList[i].Title.ToLower())
            {
                case "hunter":
                    btnDownHunter.Click();
                    btnUpFarmer.Click();
                    Helper.updateResources(_driver);
                    break;
                case "woodcutter":
                    btnDownWoodcutter.Click();
                    btnUpFarmer.Click();
                    Helper.updateResources(_driver);
                    break;
                case "miner":
                    btnDownMiner.Click();
                    btnUpFarmer.Click();
                    Helper.updateResources(_driver);
                    break;
                case "scholar":
                    btnDownScholar.Click();
                    btnUpFarmer.Click();
                    Helper.updateResources(_driver);
                    break;
            }
        }

        //Lazy switch to increment jobs
        private void jobUp(int i)
        {
            switch (_jobList[i].Title.ToLower())
            {
                case "hunter":
                    btnUpHunter.Click();
                    Helper.updateResources(_driver);
                    break;
                case "woodcutter":
                    btnUpWoodcutter.Click();
                    Helper.updateResources(_driver);
                    break;
                case "miner":
                    btnUpMiner.Click();
                    Helper.updateResources(_driver);
                    break;
                case "scholar":
                    btnUpScholar.Click();
                    Helper.updateResources(_driver);
                    break;
            }
        }

        //Overload to assign jobs by resource
        private void jobUp(string resource)
        {
            switch (resource)
            {
                case "catpower":
                    btnUpHunter.Click();
                    Helper.updateResources(_driver);
                    break;
                case "wood":
                    btnUpWoodcutter.Click();
                    Helper.updateResources(_driver);
                    break;
                case "minerals":
                    btnUpMiner.Click();
                    Helper.updateResources(_driver);
                    break;
                case "science":
                    btnUpScholar.Click();
                    Helper.updateResources(_driver);
                    break;
            }
        }
    }
}
