using System;
using System.Configuration;
using System.Threading.Tasks;
using PubSub.Messages;
using Rebus.Activation;
using Rebus.Config;
using Rebus.Handlers;
using Rebus.Logging;
using Rebus.Routing.TypeBased;

namespace PubSub.Subscriber1
{
    class Program
    {

        static Processor _processor;

        static string _connectionString => ConfigurationManager.ConnectionStrings["ServiceBusConnection"]?.ConnectionString ?? "";

        static void Main()
        {
            _processor = new Processor();

            //Console.ReadLine();
            using (var activator = new BuiltinHandlerActivator())
            {
                activator.Register(() => new Handler(_processor));

                string queueName = "paulbus2";

                Configure.With(activator)
                    .Logging(l => l.ColoredConsole(minLevel: LogLevel.Warn))
                    .Transport(t => t.UseAzureServiceBus(_connectionString, queueName)
                    .AutomaticallyRenewPeekLock())
                    .Start();

                activator.Bus.Subscribe<TestMessage>().Wait();
                activator.Bus.Subscribe<StringMessage>().Wait();
                activator.Bus.Subscribe<DateTimeMessage>().Wait();
                activator.Bus.Subscribe<TimeSpanMessage>().Wait();

                activator.Bus.SendLocal(new DateTimeMessage(DateTime.Now));

                activator.Bus.Advanced.Workers.SetNumberOfWorkers( 1 );

                Console.WriteLine("This is Subscriber 1");
                Console.WriteLine("Press ENTER to quit");
                Console.ReadLine();
                Console.WriteLine("Quitting...");
            }
        }
    }

    class Handler : IHandleMessages<TestMessage>, IHandleMessages<StringMessage>, IHandleMessages<DateTimeMessage>, IHandleMessages<TimeSpanMessage>
    {
        Processor _processor;

        public Handler(Processor processor)
        {
            _processor = processor;
        }

        public async Task Handle(StringMessage message)
        {
            try
            {
                Console.WriteLine("Before add to queue");
                if (_processor == null) _processor = new Processor();
                await _processor.AddTask(new Task(
                   () => Console.WriteLine("Got string: {0}", message.Text)
                   ));
                Console.WriteLine("after add to queue");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
                Console.WriteLine("after try catch");

        }
        private async Task DoSleepAndUpdateConsole(int ms)
        {
            Console.Write("Sleep 1s");
            for (var i = 0; i < 3; i++)
            {
                await DoSleep(ms/3);
                Console.Write(".");
            }
            Console.WriteLine("done.");
        }
        private async Task DoSleep(int ms)
        {
            System.Threading.Thread.Sleep(ms);
        }

        public async Task Handle(TestMessage message)
        {
            Console.WriteLine("Got TestMessage: {0},{1}", message.Text, message.Text2);
            await DoSleepAndUpdateConsole(1000);

        }

        public async Task Handle(DateTimeMessage message)
        {
            Console.WriteLine("Got DateTime: {0} {1}", message.DateTime, message.DateTime.Millisecond);
            Console.WriteLine("Local DateTime: {0} {1}", DateTime.Now,DateTime.Now.Millisecond);
            Console.WriteLine("Time diff (ms): {0}", (DateTime.Now- message.DateTime).Milliseconds);
            await DoSleepAndUpdateConsole(1000);


        }

        public async Task Handle(TimeSpanMessage message)
        {
            Console.WriteLine("Got TimeSpan: {0}", message.TimeSpan);
            System.Threading.Thread.Sleep(1000);
        }
    }
}
