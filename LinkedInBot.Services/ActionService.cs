using LinkedInBot.Domain;
using NLog;
using OpenQA.Selenium;
using Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkedInBot.Services
{
    public class ActionService
    {
        private readonly AppSettings _config;
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private string _currentUser;
        private SeleniumObject _driver;

        public ActionService(AppSettings config)
        {
            _config = config;
        }

        public void SetCurrentUser(string user)
        {
            _currentUser = user;
        }

        public async Task LoginAsync(LinkedinLogin login)
        {
            try
            {
                _logger.Info("Trying to login...");
                var usernameElement = _driver.FindElementByIdWithoutThrow("username");
                await InsertWordSlowly(login.Username, usernameElement);
                var passwordElement = _driver.FindElementByIdWithoutThrow("password");
                await InsertWordSlowly(login.Password, passwordElement);

                _driver.ClickOnElementByXpathWaitingLoader("//button[@aria-label='Sign in']");
            }
            catch (Exception ex)
            {
                _logger.Error("Error during Login:");
                throw;
            }
        }

        internal void SetDriver(SeleniumObject driver)
        {
            _driver = driver;
        }

        public async Task InsertWordSlowly(string word, IWebElement searchBox)
        {
            var rnd = new Random();
            var randomNumber = rnd.Next(300);
            foreach (var singleLetter in word)
            {
                searchBox.SendKeys(singleLetter.ToString());
                await Task.Delay(120 + randomNumber);
            }
        }

        internal void GoToNotificationsPage()
        {
            _driver.GotToPage("notifications");
        }

        internal void GoToMessagingPage()
        {
            _driver.GotToPage("messaging");
        }

        internal void GoToJobsPage()
        {
            _driver.GotToPage("jobs");
        }

        internal void GoToMyNetworkPage()
        {
            _driver.GotToPage("mynetwork");
        }

        internal void GoToSafeZonePage()
        {
            _driver.BrowserControl().Navigate().GoToUrl("https://www.google.com/");
        }

        internal void GoToHomePage()
        {
            _driver.GotToPage("feed");
        }

        internal void GoToNewConnectionsPage(string? _jobTitle, int? page = null)
        {
            var setPage = page ?? 1;
            var pageGoTo = "search/results/people/?page="+ setPage;

            if (!string.IsNullOrWhiteSpace(_jobTitle))
            {
                pageGoTo += "&keywords=" + _jobTitle;
            }

            _driver.GotToPage(pageGoTo);
        }

        internal void GoToFollowPage()
        {
            _driver.GotToPage("mynetwork/discover-hub");
        }
        internal async Task MoveToItemAndCenterIt(IWebElement item)
        {
            try
            {
                _driver.BrowserJavascriptControl().ExecuteScript("arguments[0].scrollIntoView(true);", item);

                if (item.Location.Y > 50)
                {
                    var js2 = String.Format("window.scrollTo({0}, {1})", 0, item.Location.Y - 100);

                    _driver.BrowserJavascriptControl().ExecuteScript(js2);
                }

                await Task.Delay(2000);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                throw;
            }
        }
        internal async Task ScrollAndDelay(int waitBetweenScroll)
        {
            _driver.BrowserJavascriptControl().ExecuteScript("window.scrollBy(0,document.body.scrollHeight)", "");
            await Task.Delay(waitBetweenScroll);
            _driver.BrowserJavascriptControl().ExecuteScript("window.scrollBy(0,document.body.scrollHeight)", "");
            await Task.Delay(waitBetweenScroll);
            _driver.BrowserJavascriptControl().ExecuteScript("window.scrollBy(0,document.body.scrollHeight)", "");
            await Task.Delay(waitBetweenScroll);
        }
    }
}
