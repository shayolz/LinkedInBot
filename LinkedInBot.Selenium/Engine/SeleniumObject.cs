using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using Selenium.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Selenium
{
    public class SeleniumObject
    {
        private SeleniumConfiguration _seleniumConfiguration;
        private WebDriverWait _wait;
        private int _defaultDelayForEachAction = 0; // we set a default delay to each action

        public SeleniumObject(SeleniumConfiguration seleniumConfiguration)
        {
            _seleniumConfiguration = seleniumConfiguration;
            _wait = new WebDriverWait(_seleniumConfiguration.Driver, TimeSpan.FromSeconds(_seleniumConfiguration.Timeout));
        }

        internal void WaitElementByXpath(string xpath, int time)
        {
            var action = By.XPath(xpath);
            WaitElement(action, time);
        }

        internal void WaitElement(By action, int time)
        {
            try
            {
                var customWait = new WebDriverWait(_seleniumConfiguration.Driver, TimeSpan.FromSeconds(time));

                // waiting the element to be visible
                customWait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(action));
                customWait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(action));
            }
            catch (Exception)
            {
                throw new Exception("element never appeared: " + action);
            }
        }

        /// <summary>
        /// Get driver
        /// </summary>
        /// <returns></returns>
        public IWebDriver BrowserControl()
        {
            return _seleniumConfiguration.Driver;
        }

        /// <summary>
        /// Get javascript driver
        /// </summary>
        /// <returns></returns>
        public IJavaScriptExecutor BrowserJavascriptControl()
        {
            return (IJavaScriptExecutor)_seleniumConfiguration.Driver;
        }

        /// <summary>
        /// Got To page, specifing the controller name
        /// </summary>
        /// <param name="Page"></param>
        public void GotToPage(string Page)
        {
            BrowserControl().Navigate().GoToUrl(_seleniumConfiguration.BaseURL + Page);
        }

        /// <summary>
        /// Check js error, call this method at the end of the operations to check browser console log
        /// </summary>
        /// <returns>Get a list of console log</returns>
        public List<string> CheckBrowserConsoleLog()
        {

            var logs = _seleniumConfiguration.Driver.Manage().Logs.GetLog(LogType.Browser).ToList();

            var results2 = new List<string>();

            foreach (var log in logs)
            {
                Console.WriteLine(log.Level + " - " + log.Message);

                if (!log.Message.Contains("Possibly unhandled rejection: undefined") &&
                   !log.Message.Contains("heres.js")) // chat bot che non funziona bene su test
                    results2.Add(log.Message);
            }

            //// actually is bugged on last realese
            //// it's fixed on selenium 4 alpha
            return results2;
        }

        /// <summary>
        /// Wait all loaders to be hidden
        /// </summary>
        private void WaitAllLoaderToBeHidder()
        {
            var customWait = new WebDriverWait(_seleniumConfiguration.Driver, TimeSpan.FromSeconds(10));

            foreach (var loader in _seleniumConfiguration.ListLoaderElementId)
            {
                try
                {
                    var lodingModals = BrowserControl().FindElements(By.XPath(loader));

                    foreach (var loadingModal in lodingModals)
                    {
                        if (loadingModal.Displayed)
                            customWait.Until(
                                SeleniumExtras.WaitHelpers.ExpectedConditions.InvisibilityOfElementLocated(
                                    By.XPath(loader)));
                    }
                }
                catch (NoSuchElementException)
                {
                    // we dont do nothing if the element is not preset on the page
                }
                catch (StaleElementReferenceException)
                {
                    // we dont do nothing if the element is not preset on the page
                    // this can happend when a loading element is generated dynamically and then removed from the page
                }
            }
        }

        /// <summary>
        /// Check if some popup are shoed
        /// </summary>
        /// <returns></returns>
        private bool CheckAllPopupAreHiddenAreHidden()
        {
            foreach (var loader in _seleniumConfiguration.ListPopupElementId)
            {
                try
                {
                    var lodingModal = BrowserControl().FindElement(By.Id(loader));

                    if (lodingModal.Displayed)
                        return false;
                }
                catch (NoSuchElementException)
                {
                    // we dont do nothing if the element is not preset on the page
                }
            }

            return true;
        }

        /// <summary>
        /// Click element by Id
        /// </summary>
        /// <param name="Id"></param>
        public void ClickOnElementByIdWaitingLoader(string Id)
        {
            var action = By.Id(Id);
            DoClickAction(action);
        }

        /// <summary>
        /// Click element by class name
        /// </summary>
        /// <param name="Class"></param>
        public void ClickOnElementByClassWaitingLoader(string Class)
        {
            var action = By.ClassName(Class);
            DoClickAction(action);
        }

        internal void WaitUntilXPathElementIsDeleted(string stringXPath)
        {
            _wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.InvisibilityOfElementLocated(By.XPath(stringXPath)));
        }

        internal void WaitAllChildsElementsAreLoaded(string xpathString)
        {
            By locator = By.XPath(xpathString);
            _wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(locator));
            _wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.VisibilityOfAllElementsLocatedBy(locator));
        }

        /// <summary>
        /// Click element by xpath
        /// </summary>
        /// <param name="Path"></param>
        public void ClickOnElementByXpathWaitingLoader(string Path)
        {
            var action = By.XPath(Path);
            DoClickAction(action);
        }

        /// <summary>
        /// Click element by Id returning null if it doens't exist
        /// </summary>
        /// <param name="Id"></param>
        public IWebElement FindElementByIdWithoutThrow(string Id)
        {
            var action = By.Id(Id);
            return FindElement(action);
        }

        /// <summary>
        /// Click element by class name returning null if it doens't exist
        /// </summary>
        /// <param name="Class"></param>
        public IWebElement FindElementByClassWithoutThrow(string Class)
        {
            var action = By.ClassName(Class);
            return FindElement(action);
        }

        /// <summary>
        /// Click element by xpath returning null if it doens't exist
        /// </summary>
        /// <param name="Path"></param>
        public IWebElement FindElementByXpathWithoutThrow(string Path)
        {
            var action = By.XPath(Path);
            return FindElement(action);
        }

        /// <summary>
        /// Finding an element, if it doesn't exist we return null
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public IWebElement FindElement(By action)
        {
            try
            {
                // wait until the element exist on page
                _wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(action));

                var element = BrowserControl().FindElement(action);
                return element;
            }
            catch (NoSuchElementException)
            {
                // if the element is not present on the page we return null
                // we don't throw
                return null;
            }
            catch (WebDriverTimeoutException)
            {
                // if the element is not present on the page we return null
                // we don't throw
                return null;
            }
        }

        /// <summary>
        /// Move to element
        /// </summary>
        /// <param name="item"></param>
        private void MoveToItemAndCenterIt(IWebElement item)
        {
            try
            {
                BrowserJavascriptControl().ExecuteScript("arguments[0].scrollIntoView(true);", item);

                if (item.Location.Y > 50)
                {
                    var js2 = String.Format("window.scrollTo({0}, {1})", 0, item.Location.Y - 100);

                    BrowserJavascriptControl().ExecuteScript(js2);
                }

                //Thread.Sleep(2000);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Click on element by action
        /// It will try three different type of click
        /// </summary>
        /// <param name="action"></param>
        private void DoClickAction(By action)
        {
            Thread.Sleep(_defaultDelayForEachAction);

            // waiting loaders
            this.WaitAllLoaderToBeHidder();

            // check if popup opened, if yes throw an error
            if (!CheckAllPopupAreHiddenAreHidden())
                throw new Exception("It's not possible to click, some popup are opened.");

            try
            {
                // waiting the element to be visible
                _wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(action));
                _wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(action));
                _wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.PresenceOfAllElementsLocatedBy(action));
                _wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.VisibilityOfAllElementsLocatedBy(action));
                _wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(action));


                TryToClickWithTypeOne(action);
            }
            catch (Exception)
            {
                try
                {
                    TryToClickWithTypeThree(action);
                }
                catch (Exception)
                {
                    try
                    {
                        TryToClickWithTypeTwo(action);
                    }
                    catch (Exception ex2)
                    {
                        throw ex2;
                    }
                }
            }

            Thread.Sleep(_defaultDelayForEachAction);
        }

        /// <summary>
        /// Third type of method to click on element using javascript
        /// </summary>
        /// <param name="action"></param>
        private void TryToClickWithTypeThree(By action)
        {
            BrowserJavascriptControl().ExecuteScript("return arguments[0].click();", BrowserControl().FindElement(action));
        }

        /// <summary>
        /// Second type of method to click on element using Actions
        /// </summary>
        /// <param name="action"></param>
        private void TryToClickWithTypeTwo(By action)
        {
            Actions actions = new Actions(_seleniumConfiguration.Driver);
            actions.MoveToElement(BrowserControl().FindElement(action));
            actions.Click(BrowserControl().FindElement(action));
        }

        /// <summary>
        /// First type of method to click on element
        /// </summary>
        /// <param name="action"></param>
        private void TryToClickWithTypeOne(By action)
        {
            var element = BrowserControl().FindElement(action);
            MoveToItemAndCenterIt(element);
            element.Click();
        }

        /// <summary>
        /// Check that the page is loaded completly
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="timeoutSec"></param>
        public void WaitDocumentReady()
        {
            _wait.Until(wd => BrowserJavascriptControl().ExecuteScript("return document.readyState").ToString() == "complete");
        }

        public void WaitInSeconds(int seconds)
        {
            Thread.Sleep(seconds * 1000);
        }

        public WebDriverWait GetWaiter()
        {
            return _wait;
        }
    }
}
