using LinkedInBot.AI;
using LinkedInBot.Domain;
using LinkedInBot.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinkedInBot.Utils;
using NLog;

namespace LinkedInBot
{
    class Program
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true);

            var appConfigs = builder.Build();
            var appConfig = appConfigs.GetSection("AppSettings").Get<AppSettings>();

            // Setting up nlog
            Nlog.SetUpLogging();

            // Cleaning up all chrome driver
            Chrome.KillAllChromeDrivers();

            // Running each account
            var tasks = (appConfig.Accounts.Select(account => RunBotAsync(account, appConfig))).ToList();

            try
            {
                Task.WaitAll(tasks.ToArray());
            }
            catch (AggregateException ae)
            {
                foreach (var item in ae.Flatten().InnerExceptions)
                {
                    _logger.Error(item);
                }
            }
        }

        static async Task RunBotAsync(LinkedinLogin login, AppSettings config)
        {
            _logger.Info("Starting bot with username: " + login.Username);
            var actionService = new ActionService(config);
            var behaviourService = new BehaviourService(actionService, config);
            var LinkedInBot = new BrainAI(login, config, behaviourService);
            await LinkedInBot.Run();
        }
  
    }
}
