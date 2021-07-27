namespace LinkedInBot.Domain
{
    public enum NextAction
    {
        NO_CONNECTION,
        TIME_TO_SLEEP,
        READ_FEED,
        ADD_NEW_USERS,
        READ_FEED_AND_PUT_LIKES,
        READ_NOTIFICATIONS,
        SEARCHING_JOB,
        READ_MESSAGES,
        ADD_COMPETENCES_TO_USER,
        LOGIN
    }
}
