namespace Nedis.Tests
{
    using NBehave.Spec.NUnit;
    using NUnit.Framework;

    [TestFixture]
    public class QueueManagerTest
    {
        [Test]
        public void Should_add_item_at_queue()
        {
            var queueManager = new QueueManager();
            queueManager.Enqueue("purchase", 1);

            queueManager.Dequeue("purchase").ShouldEqual(1);
        }
    }
}
