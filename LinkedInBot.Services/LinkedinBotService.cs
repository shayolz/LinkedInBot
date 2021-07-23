using LinkedInBot.AI;
using LinkedInBot.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkedInBot.Services
{
    public class LinkedinBotService
    {
        private readonly LinkedinLogin _login;
        private readonly AppSettings _config;

        public LinkedinBotService(LinkedinLogin login, AppSettings config)
        {
            _login = login;
            _config = config;
        }


        public void Run()
        {
            if(string.IsNullOrWhiteSpace(_login.Name) || string.IsNullOrWhiteSpace(_login.Password) || string.IsNullOrWhiteSpace(_login.Username))
            {
                throw new InvalidOperationException("Account is not correctly configured on appsettings.json. Name, username or password is missing.");
            }

            BrainAI brainAI = new BrainAI(_login, _config);
            brainAI.Start();

            brainAI.UserLogin();
        }
    }
}
