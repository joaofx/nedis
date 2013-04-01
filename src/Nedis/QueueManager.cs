namespace Nedis
{
    using System;
    using ServiceStack.Redis;

    public class QueueManager
    {
        public void Enqueue(string queueName, int id)
        {
            using (var client = new RedisClient("localhost"))
            {
                client.Lists[queueName].Prepend(id.ToString());
            }
        }

        public int Dequeue(string queueName)
        {
            using (var client = new RedisClient("localhost"))
            {
                return Convert.ToInt32(client.Lists[queueName].Pop());
            }
        }
    }
}