using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Scraping
{
    class TestCase
    {
        static void Main(string[] args)
        {
            // Download: https://sites.google.com/a/chromium.org/chromedriver/downloads
            IWebDriver driver = new ChromeDriver(@"C:\Program Files (x86)\ChromeDriver\");
            driver.Url = "http://www.demoqa.com";
        }
    }
}
