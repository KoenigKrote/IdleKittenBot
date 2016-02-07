using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.PageObjects;
using OpenQA.Selenium.Interactions;
using System.Text.RegularExpressions;
using System.Threading;
using System.IO;

namespace IdleKittenAuto.WebPage
{
    public class MainPage
    {
        public IWebDriver _driver;
        public List<Resource> _resourceList = new List<Resource>();
        private Regex rateRegex = new Regex(@"([0-9]*\.[0-9]*)");
        private Actions _builder;
        private string SaveData = string.Empty;

        [FindsBy(How = How.ClassName, Using = @"activeTab")]
        private IWebElement activeTab;

        [FindsBy(How = How.XPath, Using = @"//*[@id='resContainer']/table")]
        private IWebElement resourceTable;

        [FindsBy(How = How.XPath, Using = @"//*[@id='gameContainerId']/div[2]/div/div/table[12]/tr/td[1]/div[1]/div/span")]
        private IWebElement btnGetCatnip;

        //[FindsBy(How = How.XPath, Using = @"//*[@id='gameContainerId']/div[2]/div/div/table[12]/tr/td[1]/div[2]/div/span")]
        [FindsBy(How = How.XPath, Using = @"//*[@id='gameContainerId']/div[2]/div/div/table[12]/tr/td[1]/div[2]")]
        private IWebElement btnBuildCatnipField;

        [FindsBy(How = How.XPath, Using = @"//*[@id='gameContainerId']/div[2]/div/div/table[12]/tr/td[2]/div[1]/div/span")]
        private IWebElement btnRefineCatnip;

        [FindsBy(How = How.XPath, Using = @"//*[@id='gameContainerId']/div[2]/div/div/table[12]/tr/td[2]/div[2]/div/span")]
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


        
        public void MainLoop()
        {
            //FirefoxProfile profile = new FirefoxProfile(@"C:\Users\T\AppData\Roaming\Mozilla\Firefox\Profiles\hpgf1ifm.default");
            //_driver = new FirefoxDriver(profile);
            _driver = new FirefoxDriver();
            _builder = new Actions(_driver); 
            _driver.Navigate().GoToUrl(@"http://bloodrizer.ru/games/kittens/");
            PageFactory.InitElements(_driver, this);
            loadData();

            //while (true)
            //{
            //    getPageData();
            //    gatherCatNip();
            //}
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
            //if(activeTab.Text != "All")

            IList<IWebElement> resourceRows = resourceTable.FindElements(By.XPath(@".//tr"));
            for (int i = 0; i < resourceRows.Count; i++)
            {
                if(!string.IsNullOrWhiteSpace(resourceRows[i].Text))
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
        }

        //Prototype logic for manually gathering catnip
        private void gatherCatnip()
        {
            Resource catnip = _resourceList.Where(r => r.Name.ToLower() == "catnip").First();



            while (btnBuildCatnipField.GetAttribute("class") == "btn nosel modern")
                btnBuildCatnipField.Click();

            if (catnip.PerTick.Positive != true)
            {
                for (int i = 0; i < 10; i++)
                {
                    btnGetCatnip.Click();
                    Thread.Sleep(125); //Short pause to allow the browser to catch up
                }
            }
        }

        private bool buildCatnipField()
        {
            _builder.MoveToElement(btnBuildCatnipField).Build().Perform();
            var tipText = toolTip.Text;
            tipText = tipText.Replace("\r", " ");
            tipText = tipText.Replace("\n", " ");


        }

        //Strips non-alpha characters from a given string
        private string StripNonChar(string input)
        {
            input = new string(input.Where(c => char.IsLetter(c)).ToArray());
            return input;
        }

        //Strips non-numerical characters from a given string
        private int StripNonNum(string input)
        {
            int number;
            input = new string(input.Where(c => char.IsDigit(c)).ToArray());
            int.TryParse(input, out number);
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
    }

    public class Resource
    {
        public string Name { get; set; }
        public int Amount { get; set; }
        public int MaxAmount { get; set; }
        public int MinAmount { get; set; }
        public Rate PerTick { get; set; }
    }

    public class Rate
    {
        public bool? Positive { get; set; }
        public double Delta { get; set; }
    }
}
