using System;
using System.Collections.Generic;
using LinkedInBot.Domain;
using LinkedInBot.Utils;

namespace LinkedInBot.AI
{
    public class BaseClassAI
    {
        public NextAction GetNextBehaviour()
        {
            var results = new List<NextAction>();
            var dateNow = DateTimeOffset.Now;
            if ((dateNow.Hour >= 22 || dateNow.Hour < 8) || (dateNow.Hour >= 10 && dateNow.Hour < 12) || (dateNow.Hour >= 16 && dateNow.Hour < 19))
                return NextAction.TIME_TO_SLEEP;

            var isConnectionAvailable = Connection.CheckIfNetworkConnectionIsAvailable();

            if (!isConnectionAvailable)
            {
                return NextAction.NO_CONNECTION;
            }

            return NextAction.TIME_TO_SLEEP;
        }
    }
}
