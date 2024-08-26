namespace Shared.Constants
{
    public static class Consts
    {
        public static class RedisConsts
        {
            public const string JOBSEARCH_KEY = "jobsearch";
        }
        public static class RabbitMqConsts
        {
            public const string JOBSEARCH_QUEUE = "jobsearch-queue";
            public const string BOSS_AZ_QUEUE = "boss-az-queue";
            public const string HELLOJOB_QUEUE = "hellojob-queue";
        }
    }
}
