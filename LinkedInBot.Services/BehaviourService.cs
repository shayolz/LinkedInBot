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
        private string _jobTitle;

        public BehaviourService(ActionService actionService, AppSettings config)
        {
            _actionService = actionService;
            _config = config;
        }

        public void SetCurrentUser(LinkedinLogin user)
        {
            _logger.Info("[" + user + "] Authenticated user => " + user);
            _currentUser = user.Name;
            _jobTitle = user.JobTitle;
            _actionService.SetCurrentUser(user.Name);
        }

        public void SetDriver(SeleniumObject driver)
        {
            _actionService.SetDriver(driver);
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

                await _actionService.LoginAsync(login);

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

        public async Task GoToSafeZone()
        {
            _actionService.GoToSafeZonePage();
        }

        public string GetUserName()
        {
            return _driver.FindElementByXpathWithoutThrow("//div[contains(@class, 'profile-rail-card__actor-link')]").Text;
        }

        public void Initialize()
        {
            var builder = new UITestBuilder();
            var headLess = true;

#if DEBUG
            headLess = false;
#endif


            _driver = builder
                    .SetBaseUrl("https://www.linkedin.com/")
                    .SetLogLevel(OpenQA.Selenium.LogLevel.Severe) // optional
                    .SetDisableAllBrowserExtensions(true) // optional
                    .SetDisableGpu(true) // optional
                    .SetHideInfoBar(true) // optional
                    .SetHideBrowser(headLess)
                    .SetStartWithBrowserFullWindow(true) // optional
                    .SetLoaderListId(new List<string> { "loading-bar", "artdeco-loader__bars" }) // optional
                    .SetPopupListId(new List<string> { }) // optional
                    .SetTimeout(5) // optional
                    .Create()
                    .ValidateAndRun();

            _actionService.SetDriver(_driver);
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
                    await AddNewUsers();
                    break;
                case NextAction.ADD_NEW_FOLLOW_PAGE:
                    await AddNewFollowPage();
                    break;
                case NextAction.ACCEPT_INVITATIONS:
                    await AcceptInvitations();
                    break;
                case NextAction.READ_FEED_AND_PUT_LIKES:
                    await ReadFeedAndPutLikesAsync();
                    break;
                case NextAction.READ_NOTIFICATIONS:
                    break;
                case NextAction.SEARCHING_JOB:
                    await SearchingJobs();
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

        /**
         * Behaviour AddNewFollowPage
         */
        private async Task AddNewFollowPage()
        {
            _actionService.GoToFollowPage();
            _driver.ClickOnElementByXpathWaitingLoader("//button[contains(@aria-label,'Follow ')]");
            _logger.Info("[" + _currentUser + "] Added new follow page.");
            await Task.Delay(120000);

        }

        /**
         * Behaviour AcceptInvitations
         */
        private async Task AcceptInvitations()
        {
            _actionService.GoToMyNetworkPage();
            var acceptButtons = _driver.BrowserControl().FindElements(By.XPath("//button[contains(@aria-label,'Accept ')]"));

            foreach (var button in acceptButtons)
            {
                await _actionService.MoveToItemAndCenterIt(button);
                if (button.Displayed && button.Enabled)
                {
                    button.Click();
                    _logger.Info("[" + _currentUser + "] Accepted new user request.");
                }

                await Task.Delay(10000);
            }
        }

        /**
         * Behaviour AddNewUsers
         */
        private async Task AddNewUsers()
        {
            _actionService.GoToMyNetworkPage();
            await _actionService.ScrollAndDelay(1000);
            _driver.ClickOnElementByXpathWaitingLoader("//button[contains(@aria-label,'to connect')]");
            _logger.Info("[" + _currentUser + "] Added new user.");
            await Task.Delay(120000);
        }

        /**
         * Behaviour SearchingJobs
         */
        private async Task SearchingJobs()
        {
            _actionService.GoToJobsPage();
            _driver.ClickOnElementByXpathWaitingLoader("//section[contains(@class,'job-card-container')]");
            await Task.Delay(30000);
            _driver.ClickOnElementByXpathWaitingLoader("//a[contains(@class,'job-card-container__link')]");
            await Task.Delay(120000);
        }

        /**
         * Behaviour ReadFeedAsync
         */
        private async Task ReadFeedAsync()
        {
            GoToHome();

            var readCount = 0;
            var maxReads = 3;

            while (readCount <= maxReads)
            {
                _logger.Info("[" + _currentUser + "] Scrolling feed " + DateTimeOffset.Now);

                await _actionService.ScrollAndDelay(6000);

                await Task.Delay(30000);
                readCount++;
            }
        }


        /**
         * Behaviour ReadFeedAndPutLikesAsync
         */
        private async Task ReadFeedAndPutLikesAsync()
        {
            GoToHome();

            var currentLikeCount = 0;
            var maxLikes = 3;
            while (currentLikeCount <= maxLikes)
            {
                _logger.Info("[" + _currentUser + "] Scrolling feed and putting likes " + DateTimeOffset.Now);

                await _actionService.ScrollAndDelay(2000);

                var likesIcon = _driver.BrowserControl().FindElements(By.XPath("//li-icon[contains(@type,'like-icon')]")).Count;

                if (likesIcon == 0)
                {
                    currentLikeCount++;
                    continue;
                }

                var rand = new Random();
                var randomnum = rand.Next(1, likesIcon);

                if (randomnum < 0)
                    randomnum = 1;


                _driver.ClickOnElementByXpathWaitingLoader("(//li-icon[contains(@type,'like-icon')])[" + randomnum + "]");
                _logger.Info("[" + _currentUser + "] Put new like on post.");

                await Task.Delay(10000);
                currentLikeCount++;
            }
        }


        /**
         * Behaviour BehaviourTimeToSleep
         */
        private async Task BehaviourTimeToSleeppower()
        {
            const int oneHour = 3600000;
            _logger.Info("[" + _currentUser + "] It's sleeping time, sleeping for 1 hour at " + DateTimeOffset.Now);
            _driver.BrowserControl().Navigate().GoToUrl($"https://www.google.com/");
            await Task.Delay(oneHour);
        }

        /**
        * Behaviour BehaviourNoConnection
        */
        private async Task BehaviourNoConnection()
        {
            const int fiveMinutes = 300000;
            _logger.Warn("[" + _currentUser + "] NO INTERNETCONNECTION AVAILABLE, waiting 5 minutes...");
            await Task.Delay(fiveMinutes);
        }

        public void GoToNotifications()
        {
            _actionService.GoToNotificationsPage();
        }

        public void GoToMessaging()
        {
            _actionService.GoToMessagingPage();
        }

        public void GoToJobs()
        {
            _actionService.GoToJobsPage();
        }

        public void GoToMyNetwork()
        {
            _actionService.GoToMyNetworkPage();
        }

        public void GoToNewConnections(string jobTitle)
        {
            _actionService.GoToNewConnectionsPage(jobTitle);
        }

        public void GoToHome()
        {
            _actionService.GoToHomePage();
        }

        internal void GoToFollowPage()
        {
            _actionService.GoToFollowPage();
        }
    }
}
