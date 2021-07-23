using LinkedInBot.Domain;
using System;

namespace LinkedInBot.AI
{
    public class BrainAI
    {
        private readonly LinkedinLogin _login;
        private readonly AppSettings _config;

        public BrainAI(LinkedinLogin login, AppSettings config)
        {
            _login = login;
            _config = config;
        }

        public void Start()
        {
            //TODO: something to do on start? maybe start selenium bot here and do some checks
        }

        public void UserLogin()
        {
            throw new NotImplementedException();
        }

        public void UserLogout()
        {
            throw new NotImplementedException();
        }

        public void GotToLinkedInHome()
        {
            throw new NotImplementedException();
        }

        public void GoToPersonalProfile()
        {
            throw new NotImplementedException();
        }

        public void GoToPersonalMessages()
        {
            throw new NotImplementedException();
        }

        public void GoToSearchPeople()
        {
            throw new NotImplementedException();
        }
    }
}
