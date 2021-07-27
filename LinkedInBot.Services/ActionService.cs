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

        public ActionService(AppSettings config)
        {
            _config = config;
        }

        public void SetCurrentUser(string user)
        {
            _currentUser = user;
        }

        public async Task LoginAsync(SeleniumObject _driver, LinkedinLogin login)
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
    }
}
