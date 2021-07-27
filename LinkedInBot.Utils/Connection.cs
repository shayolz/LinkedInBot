using NLog;
using System;
using System.Net;

namespace LinkedInBot.Utils
{
    public static class Connection
    {
        public static bool CheckIfNetworkConnectionIsAvailable()
        {
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead("http://google.com/generate_204"))
                    return true;
            }
            catch
            {
                return false;
            }
        }

    }
}
