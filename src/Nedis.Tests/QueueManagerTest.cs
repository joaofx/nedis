namespace Nedis.Tests
{
    using NBehave.Spec.NUnit;
    using NUnit.Framework;

    [TestFixture]
    public class QueueManagerTest
    {
        private readonly QueueManager queueManager;

        public QueueManagerTest()
        {
            this.queueManager = new QueueManager();
        }

        [Test]
        public void Should_add_item_and_retrive_from_queue()
        {
            this.queueManager.Enqueue("purchase", 1);
            this.queueManager.Enqueue("purchase", 2);

            this.queueManager.Dequeue("purchase").ShouldEqual(1);
            this.queueManager.Dequeue("purchase").ShouldEqual(2);
        }
    }
}
