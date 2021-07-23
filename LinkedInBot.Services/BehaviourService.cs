using LinkedInBot.Domain;
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

        public BehaviourService(ActionService actionService, AppSettings config)
        {
            _actionService = actionService;
            _config = config;
        }

        public void Login(LinkedinLogin login)
        {
            while (true)
            {
                Console.WriteLine("[" + login.Name + "] Trying to login...");
                var success = _actionService.Login(login);

                if (success)
                    return;
            }
        }
    }
}
