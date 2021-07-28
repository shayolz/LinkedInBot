using LinkedInBot.Domain;
using LinkedInBot.Services;
using LinkedInBot.Utils;
using NLog;
using System;
using System.Threading.Tasks;

namespace LinkedInBot.AI
{
    public class BrainAI : BaseClassAI
    {
        private readonly LinkedinLogin _login;
        private readonly AppSettings _config;
        private readonly BehaviourService _behaviourService;
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public BrainAI(LinkedinLogin login, AppSettings config, BehaviourService behaviourService)
        {
            _login = login;
            _config = config;
            _behaviourService = behaviourService;
        }


        public async Task Run()
        {
            if (string.IsNullOrWhiteSpace(_login.Password) || string.IsNullOrWhiteSpace(_login.Username))
            {
                throw new InvalidOperationException("Account is not correctly configured on appsettings.json. Name, username or password is missing.");
            }

            _behaviourService.Initialize();
            await _behaviourService.LoginAsync(_login);
            _login.Name = _behaviourService.GetUserName();
            _behaviourService.SetCurrentUser(_login);

            if (string.IsNullOrWhiteSpace(_login.Name))
            {
                throw new InvalidOperationException("Cannot find user name...");
            }

            while (true)
            {
                try
                {
                    var nextBehaviour = GetNextBehaviour();
                    if(nextBehaviour != NextAction.NO_CONNECTION && nextBehaviour != NextAction.TIME_TO_SLEEP)
                    {
                        await _behaviourService.CheckIfIsLoggedInAsync(_login);
                    }

                    await _behaviourService.HandleNextAsync(nextBehaviour);

                    await _behaviourService.GoToSafeZone();
                    await Task.Delay(_config.WaitTimeAfterEveryBehaviour * 1000);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                    _logger.Error("[" + _login.Name + "] Error on Behaviour. Waiting 5 minutes...");
                    await Task.Delay(300000);
                }

            }
        }
    }
}
