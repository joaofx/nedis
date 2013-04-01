namespace Nedis
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using ServiceStack.Redis;

    public class NedisQueue : IDisposable
    {
        private Thread subscribeThread;
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
            this.subscribeThread = new Thread(() =>
            {
                using (var client = new RedisClient("localhost"))
                {
                    using (var subscription = client.CreateSubscription())
                    {
                        subscription.OnSubscribe =
                            channel => Debug.WriteLine(string.Format("Subscribed to '{0}'", channel));

                        subscription.OnUnSubscribe =
                            channel => Debug.WriteLine(string.Format("UnSubscribed from '{0}'", channel));

                        subscription.OnMessage = (channel, msg) =>
                        {
                            Debug.WriteLine(string.Format("Received '{0}' from channel '{1}' Busy: {2}", msg, channel, false));
                            action();
                        };

                        subscription.SubscribeToChannels(queueName);

                        Debug.WriteLine("Subscribed");
                    }
                }
            });

            this.subscribeThread.Start();
        }

        public void Dispose()
        {
            this.subscribeThread.Abort();
        }
    }
}