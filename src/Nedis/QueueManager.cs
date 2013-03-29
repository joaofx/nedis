namespace Nedis
{
    using ServiceStack.Redis;

    public class QueueManager
    {
        public void Enqueue(string queueName, int id)
        {
            using (var redisClient = new RedisClient("localhost"))
            {
            }
        }

        public int Dequeue(string queueName)
        {
            using (var redisClient = new RedisClient("localhost"))
            {
                return 1;
            }
        }
    }
}