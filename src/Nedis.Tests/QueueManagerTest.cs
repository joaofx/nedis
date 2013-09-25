namespace Nedis.Tests
{
    using System.Threading;
    using NBehave.Spec.NUnit;
    using NUnit.Framework;
    using Rhino.Mocks;

    [TestFixture]
    public class QueueManagerTest
    {
        private NedisQueue nedisQueue;
        private IExecutable executable;

        [SetUp]
        public void BeforeTest()
        {
            SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
            this.nedisQueue = new NedisQueue();
            this.executable = MockRepository.GenerateMock<IExecutable>();

            this.nedisQueue.Clear("to_index");
        }

        [Test]
        public void Should_add_item_and_retrive_from_queue()
        {
            this.nedisQueue.Enqueue("to_index", 1);
            this.nedisQueue.Enqueue("to_index", 2);

            this.nedisQueue.Dequeue("to_index").ShouldEqual(1);
            this.nedisQueue.Dequeue("to_index").ShouldEqual(2);
        }

        [Test]
        public void Should_receive_notification_when_item_was_added()
        {
            this.nedisQueue.Subscribe("to_index", () => this.executable.Execute());

            this.nedisQueue.Enqueue("to_index", 1);
            this.nedisQueue.Enqueue("to_index", 2);

            this.executable.AssertWasCalled(
                x => x.Execute(),
                x => x.Repeat.Twice());

            Thread.Sleep(10000);
        }

        public interface IExecutable
        {
            void Execute();
        }
    }
}
