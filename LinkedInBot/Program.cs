using Microsoft.Extensions.Configuration;
using System;

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
        }
    }
}
