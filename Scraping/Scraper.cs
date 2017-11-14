using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Scraping
{
    public class Scraper : IDisposable
    {
        public static string DRIVER_PATH = @"C:\Program Files (x86)\ChromeDriver\";
        private const int SECONDS = 1000;


        private IWebDriver m_driver;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        public Scraper(string url)
        {

            m_driver = new ChromeDriver(DRIVER_PATH);



            m_driver.Url = url;

            List<string> neededFilters = new List<string>
            {
                "Accetta carte di credito",
                "Cucina locale"
            };

            //foreach (var filter in neededFilters)
            //{
            //    var toClick = getClickableFilter(filter);
            //}
            var toClick = getClickableFilter(neededFilters[0]);
            if (toClick.Count <= 0)
                Log("No filters found.", true);
        }

        /// <summary>
        /// Alternative way to get filters by name
        /// </summary>
        /// <param name="name">Name of the filter</param>
        /// <returns></returns>
        private IWebElement getClickableFilter(string name)
        {
            var filters = m_driver.FindElements(By.CssSelector("div.label.filterName"));
            foreach(var f in filters)
            {
                var link = SafeFindElement(f, By.XPath("//a")).Text;
                var notLink = SafeGetText(f);

                if (name.Equals(link) || name.Equals(notLink))
                {
                    var tmp = SafeFindElement(f, By.XPath(".."));
                    Log(tmp.ToString());
                    return tmp;
                }
            }
            return null;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterName"></param>
        /// <returns></returns>
        private IReadOnlyCollection<IWebElement> g(string filterName)
        {
            List<IWebElement> foundFilter = new List<IWebElement>();
            // wait for page to load
            Thread.Sleep(3 * SECONDS);
            // look for filters
            var foundFilters = m_driver.FindElements(By.CssSelector(".filterItem"));
            string previousName = "";
            foreach (var f in foundFilters)
            {
                bool hasInnerLink = false;
                var filterItem = f;
                var textName = string.Empty;
                // If dosen't contain filter name
                if ((textName = tryToGetText(filterItem)).Equals(""))
                {
                    IWebElement nameElement;

                    // get it from one of the link inside the div child
                    nameElement = SafeFindElement(filterItem, By.CssSelector(".label.filterName a"));
                    textName = SafeGetAttribute(nameElement, "innerHTML");

                    if (string.IsNullOrEmpty(textName))
                    {
                        // get it from one of the text of the child div
                        m_driver.Navigate().GoToUrl(m_driver.Url);
                        nameElement = SafeFindElement(filterItem, By.XPath("//div[@class='filterAddOn']/div[contains(text(), 'Leggi di più')]"));
                        var toExpandElelemnt = SafeFindElement(nameElement, By.XPath(".."));
                        toExpandElelemnt.Click();
                        Thread.Sleep(1 * SECONDS);

                        nameElement = SafeFindElement(filterItem, By.XPath("//div[@class='label']"));
                        textName = SafeGetAttribute(nameElement, "innerHTML");
                    }
                }

                previousName = textName;
                Log(textName);
                if (textName.Equals(filterName))
                    if (!textName.Equals(previousName))
                        foundFilter.Add(f);
            }

            return new ReadOnlyCollection<IWebElement>(foundFilter);
        }

        #region Helpers

        public static void Log(string message, bool isError = false)
        {
            var prefix = "Debug: ";
            if (isError)
                prefix = "\n###### Error: ";

            Console.WriteLine(prefix + message);
        }

        private string tryToGetText(IWebElement elem)
        {
            try
            {
                return elem.Text;
            }
            catch (Exception e)
            {
                Log(e.Message, true);
                return "";
            }
        }

        /// <summary>
        /// Uses the <see cref="IWebDriver.FindElement"/> method,
        /// but it handles the <see cref="NoSuchElementException"/>
        /// </summary>
        /// <param name="parent">The WebElement you want to start the search from</param>
        /// <param name="type">The By selector required for the search</param>
        /// <returns>The element found or null</returns>
        private IWebElement SafeFindElement(IWebElement parent, By type)
        {
            IWebElement child = null;
            try
            {
                // get it from one of the text of the child div
                child = parent.FindElement(type);
            }
            catch (NoSuchElementException e)
            {
                Log(e.Message, true);
            }

            return child;
        }

        /// <summary>
        /// Gets attribute from an element managing any exception thrown
        /// </summary>
        /// <param name="element">Element to get the attribute from</param>
        /// <param name="attributeName">Attribute name</param>
        /// <returns></returns>
        private string SafeGetAttribute(IWebElement element, string attributeName)
        {
            string atttributeValue = "";
            try
            {
                atttributeValue = element.GetAttribute(attributeName);
            }
            catch (Exception e)
            {
                Log(e.Message, true);
            }

            return atttributeValue;
        }

        /// <summary>
        /// Gets text from an element managing any exception thrown
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private string SafeGetText(IWebElement element)
        {
            string text = "";
            try
            {
                text = element.Text;
            }
            catch (Exception e)
            {
                Log(e.Message, true);
            }

            return text;
        }

        #endregion

        public void Dispose()
        {
            m_driver?.Dispose();
        }
    }
}

//jfy_filter_bar_establishmentTypeFilters > 
//    div.filterContent.ui_label_group.inline > 
//        div.ui_input_checkbox.filterItem.lhrFilter.filter.establishmentTypeFilters.establishmentTypeFilters_9901.\33.index_3 > 
//            div > 
//              a


//IWebElement demoDiv = driver.FindElement(By.Id("demo-div"));

//Console.WriteLine(demoDiv.GetAttribute("innerHTML"));
//Console.WriteLine(driver.ExecuteJavaScript<string>("return arguments[0].innerHTML", demoDiv));

//Console.WriteLine(demoDiv.GetAttribute("textContent"));
//Console.WriteLine(driver.ExecuteJavaScript<string>("return arguments[0].textContent", demoDiv));

// #overlayInnerDiv > div.filtersOverlayBody.withSearch.ui_label_group.inline > div.filters > div:nth-child(1) > div:nth-child(1) > div