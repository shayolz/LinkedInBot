using LinkedInBot.Domain;
using LinkedInBot.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinkedInBot
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true);

            var appConfigs = builder.Build();
            var appConfig = appConfigs.GetSection("AppSettings").Get<AppSettings>();


            Parallel.ForEach(appConfig.Accounts, (a) => RunBotAsync(a, appConfig));

        }

        static void RunBotAsync(LinkedinLogin login, AppSettings config)
        {
            LinkedinBotService IgBot = new LinkedinBotService(login, config);
            IgBot.Run();
        }

   
    }
}
