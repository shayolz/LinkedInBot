using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace LinkedInBot.Utils
{
    public static class Chrome
    {
        /*
         * Cleaning up chrome driver process that can remain append
         */
        public static void KillAllChromeDrivers()
        {
            foreach (var process in Process.GetProcessesByName("chromedriver"))
            {
                process.Kill();
            }
        }
    }
}
