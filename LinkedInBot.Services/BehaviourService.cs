using LinkedInBot.Domain;
using NLog;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkedInBot.Services
{
    public class BehaviourService
    {
        private readonly ActionService _actionService;
        private readonly AppSettings _config;
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private SeleniumObject _driver;
        private string _currentUser;

        public BehaviourService(ActionService actionService, AppSettings config)
        {
            _actionService = actionService;
            _config = config;
        }

        public void SetCurrentUser(string user)
        {
            _logger.Info("[" + user + "] Authenticated user => " + user);
            _currentUser = user;
            _actionService.SetCurrentUser(user);
        }

        public async Task LoginAsync(LinkedinLogin login)
        {
            while (true)
            {
                _driver.GotToPage("login/en");

                var currentURL = _driver.BrowserControl().Url;

                if (!currentURL.Contains("login"))
                {
                    return;
                }

                await _actionService.LoginAsync(_driver, login);

                _driver.GotToPage("feed");

                currentURL = _driver.BrowserControl().Url;

                if (currentURL.Contains("feed"))
                {
                    return;
                }
                else
                {
                    _logger.Warn("Login Failed, waiting 2 minutes...");
                    await Task.Delay(120000);
                }
            }
        }

        public async Task CheckIfIsLoggedInAsync(LinkedinLogin login)
        {
            _driver.GotToPage("feed");

            var currentURL = _driver.BrowserControl().Url;

            if (currentURL.Contains("feed"))
            {
                return;
            }

            await LoginAsync(login);
        }

        public string GetUserName()
        {
            return _driver.FindElementByXpathWithoutThrow("//div[contains(@class, 'profile-rail-card__actor-link')]").Text;
        }

        public void Initialize()
        {
            var builder = new UITestBuilder();

            _driver = builder
                    .SetBaseUrl("https://www.linkedin.com/")
                    .SetLogLevel(OpenQA.Selenium.LogLevel.Severe) // optional
                    .SetDisableAllBrowserExtensions(true) // optional
                    .SetDisableGpu(true) // optional
                    .SetHideInfoBar(true) // optional
                    .SetHideBrowser(false)
                    .SetStartWithBrowserFullWindow(true) // optional
                    .SetLoaderListId(new List<string> { "loading-bar", "artdeco-loader__bars" }) // optional
                    .SetPopupListId(new List<string> { }) // optional
                    .SetTimeout(120) // optional
                    .Create()
                    .ValidateAndRun();
        }

        public async Task HandleNextAsync(NextAction nextAction)
        {
            _logger.Info("[" + _currentUser + "] Next action => " + nextAction.ToString());
            switch (nextAction)
            {
                case NextAction.NO_CONNECTION:
                    await BehaviourNoConnection();
                    break;
                case NextAction.TIME_TO_SLEEP:
                    await BehaviourTimeToSleep();
                    break;
                case NextAction.READ_FEED:
                    await ReadFeedAsync();
                    break;
                case NextAction.ADD_NEW_USERS:
                    break;
                case NextAction.READ_FEED_AND_PUT_LIKES:
                    await ReadFeedAndPutLikesAsync();
                    break;
                case NextAction.READ_NOTIFICATIONS:
                    break;
                case NextAction.SEARCHING_JOB:
                    break;
                case NextAction.READ_MESSAGES:
                    break;
                case NextAction.ADD_COMPETENCES_TO_USER:
                    break;
                case NextAction.LOGIN:
                    break;
                default:
                    break;
            }
        }

        private Task ReadFeedAsync()
        {
            throw new NotImplementedException();
        }

        private async Task ReadFeedAndPutLikesAsync()
        {
            var currentLikeCount = 0;
            var maxLikes = 3;
            while (true)
            {
                if (currentLikeCount > maxLikes)
                    break;

                await ScrollAndDelay();

                var allUserList = _driver.BrowserControl().FindElements(By.XPath("//li-icon[contains(@type,'like-icon')]")).Count;

                var rand = new Random();
                var randomnum = rand.Next(0, allUserList);

                if (randomnum < 0)
                    randomnum = 0;

                try
                {
                    var chooseOneUserToFollow = _driver.BrowserControl().FindElement(By.XPath("(//li-icon[contains(@type,'like-icon')])[" + randomnum + "]"));


                    Actions actions = new Actions(_driver.BrowserControl());
                    actions.MoveToElement(chooseOneUserToFollow);
                    actions.Perform();
                    await Task.Delay(1000);

                    await MoveToItemAndCenterIt(chooseOneUserToFollow);


                    if (chooseOneUserToFollow.Displayed && chooseOneUserToFollow.Enabled)
                    {
                        chooseOneUserToFollow.Click();
                        //instaHelper.ListFollowersAddDate.Add(DateTime.Now);
                    }

                    await Task.Delay(3000);

                }
                catch (Exception ex)
                {
                    // dont do nothing for now we don't care
                    throw;
                }

                await Task.Delay(4000);
                currentLikeCount++;
            }
        }

        public async Task ScrollAndDelay()
        {
            _driver.BrowserJavascriptControl().ExecuteScript("window.scrollBy(0,document.body.scrollHeight)", "");
            await Task.Delay(1000);
            _driver.BrowserJavascriptControl().ExecuteScript("window.scrollBy(0,document.body.scrollHeight)", "");
            await Task.Delay(1000);
            _driver.BrowserJavascriptControl().ExecuteScript("window.scrollBy(0,document.body.scrollHeight)", "");
            await Task.Delay(1000);
        }

        private async Task MoveToItemAndCenterIt(IWebElement item)
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

        private async Task BehaviourTimeToSleep()
        {
            const int oneHour = 3600000;
            _logger.Info("[" + _currentUser + "] It's sleeping time, sleeping for 1 hour at "+DateTimeOffset.Now);
            _driver.BrowserControl().Navigate().GoToUrl($"https://www.google.com/");
            await Task.Delay(oneHour);
        }

        private async Task BehaviourNoConnection()
        {
            const int fiveMinutes = 300000;
            _logger.Warn("[" + _currentUser + "] NO INTERNETCONNECTION AVAILABLE, waiting 5 minutes...");
            await Task.Delay(fiveMinutes);
        }
        public void GoToNotifications()
        {
            throw new NotImplementedException();
        }

        public void GoToMessaging()
        {
            throw new NotImplementedException();
        }

        public void GoToJobs()
        {
            throw new NotImplementedException();
        }

        public void GoToMyNetwork()
        {
            throw new NotImplementedException();
        }

        public void GoToHome()
        {
            throw new NotImplementedException();
        }
    }

  

}
