using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Scraping
{
    class TestCase
    {
        // Download: https://sites.google.com/a/chromium.org/chromedriver/downloads
        private static string Domain { get; } ="https://www.tripadvisor.it";
        static void Main(string[] args)
        {
            //TODO: use args to get url
            string url = "/Restaurants-g187855-Turin_Province_of_Turin_Piedmont.html";
            Scraper scraper = new Scraper(Domain + url);
        }
    }
}
