namespace Nedis
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using ServiceStack.Redis;

    public class NedisQueue
    {
        private const string NewItem = "NewItem";

        public void Enqueue(string queueName, int id)
        {
            using (var client = new RedisClient("localhost"))
            {
                client.Lists[queueName].Prepend(id.ToString());
            }

            this.NotifySubscribers(queueName);
        }

        private void NotifySubscribers(string queueName)
        {
            using (var client = new RedisClient("localhost"))
            {
                client.PublishMessage(queueName, NewItem);
            }
        }

        public int Dequeue(string queueName)
        {
            using (var client = new RedisClient("localhost"))
            {
                return Convert.ToInt32(client.Lists[queueName].Pop());
            }
        }

        public void Subscribe(string queueName, Action action)
        {
            Task.Factory.StartNew(() =>
            {
                using (var client1 = new RedisClient("localhost"))
                {
                    using (var subscription1 = client1.CreateSubscription())
                    {
                        subscription1.OnSubscribe =
                            channel => Debug.WriteLine(string.Format("Subscribed to '{0}'", channel));

                        subscription1.OnUnSubscribe =
                            channel => Debug.WriteLine(string.Format("UnSubscribed from '{0}'", channel));

                        subscription1.OnMessage = (channel, msg) =>
                        {
                            Debug.WriteLine(string.Format("Received '{0}' from channel '{1}' Busy: {2}", msg, channel, false));
                            action();
                        };

                        subscription1.SubscribeToChannels(queueName);

                        Debug.WriteLine("Subscribed");
                    }
                }
            }).Start();
        }

        public void Clear(string queueName)
        {
            using (var client = new RedisClient("localhost"))
            {
                client.Lists[queueName].Clear();
            }
        }
    }
}