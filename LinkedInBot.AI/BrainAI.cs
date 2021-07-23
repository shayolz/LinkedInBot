using LinkedInBot.Domain;
using LinkedInBot.Services;
using System;

namespace LinkedInBot.AI
{
    public class BrainAI
    {
        private readonly LinkedinLogin _login;
        private readonly AppSettings _config;
        private readonly BehaviourService _behaviourService;

        public BrainAI(LinkedinLogin login, AppSettings config, BehaviourService behaviourService)
        {
            _login = login;
            _config = config;
            _behaviourService = behaviourService;
        }


        public void Run()
        {
            if (string.IsNullOrWhiteSpace(_login.Name) || string.IsNullOrWhiteSpace(_login.Password) || string.IsNullOrWhiteSpace(_login.Username))
            {
                throw new InvalidOperationException("Account is not correctly configured on appsettings.json. Name, username or password is missing.");
            }

            _behaviourService.Login(_login);
        }
    }
}
