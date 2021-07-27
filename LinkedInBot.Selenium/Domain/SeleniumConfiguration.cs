using OpenQA.Selenium;
using System.Collections.Generic;

namespace Selenium.Domain
{
    public class SeleniumConfiguration
    {
        public IWebDriver Driver { get; set; }
        public IEnumerable<string> ListPopupElementId;
        public IEnumerable<string> ListLoaderElementId;
        public int Timeout;
        public string BaseURL;
    }
}
