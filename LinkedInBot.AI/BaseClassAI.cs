using System;
using LinkedInBot.Domain;
using LinkedInBot.Utils;

namespace LinkedInBot.AI
{
    public class BaseClassAI
    {
        public static NextAction GetNextBehaviour()
        {
            NextAction result = NextAction.TIME_TO_SLEEP;
            var dateNow = DateTimeOffset.Now;
            if (dateNow.Hour >= 22 || dateNow.Hour < 8 || (dateNow.Hour >= 10 && dateNow.Hour < 12) || (dateNow.Hour >= 15 && dateNow.Hour < 20))
                return NextAction.TIME_TO_SLEEP;

            var isConnectionAvailable = Connection.CheckIfNetworkConnectionIsAvailable();

            if (!isConnectionAvailable)
            {
                return NextAction.NO_CONNECTION;
            }

            Random rnd = new Random();
            int randomBehaviour = rnd.Next(1, 7);

            switch (randomBehaviour)
            {
                case 1:
                    result = NextAction.READ_FEED;
                    break;
                case 2:
                    result = NextAction.ADD_NEW_USERS;
                    break;
                case 3:
                    result = NextAction.ACCEPT_INVITATIONS;
                    break;
                case 4:
                    result = NextAction.READ_FEED_AND_PUT_LIKES;
                    break;
                case 5:
                    result = NextAction.SEARCHING_JOB;
                    break;
                case 6:
                    result = NextAction.ADD_NEW_FOLLOW_PAGE;
                    break;
                default:
                    break;
            }

            return result;
        }
    }
}
