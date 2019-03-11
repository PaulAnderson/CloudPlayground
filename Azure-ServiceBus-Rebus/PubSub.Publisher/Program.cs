using System;
using System.Configuration;
using System.IO;
using PubSub.Messages;
using Rebus.Activation;
using Rebus.Config;
using Rebus.Logging;
using Rebus.Persistence.FileSystem;
using Rebus.Persistence.InMem;
using Rebus.Routing.TypeBased;

namespace PubSub.Publisher
{
    class Program
    {
        static string _connectionString => ConfigurationManager.ConnectionStrings["ServiceBusConnection"]?.ConnectionString ?? "";

        static void Main()
        {
            string queueName = "paulbus2";

            using (var activator = new BuiltinHandlerActivator())
            {
                Configure.With(activator)
                    .Logging(l => l.ColoredConsole(minLevel: LogLevel.Warn))
                    .Transport(t => t.UseAzureServiceBusAsOneWayClient(_connectionString))
                    .Routing(r => r.TypeBased().MapFallback("paulbus2"))
                     .Options(o =>
                     {
                         o.SetNumberOfWorkers(1);
                         o.SetMaxParallelism(1);
                     })
                    .Start();

                var startupTime = DateTime.Now;

                while (true)
                {
                    Console.WriteLine(@"a) Publish string
b) Publish DateTime
c) Publish TimeSpan
q) Quit");

                    var keyChar = char.ToLower(Console.ReadKey(true).KeyChar);

                    switch (keyChar)
                    {
                        case 'a':
                            activator.Bus.Publish(new StringMessage("Hello there, I'm a publisher!")).Wait();
                            break;

                        case 'b':
                            activator.Bus.Publish(new DateTimeMessage(DateTime.Now)).Wait();
                            break;

                        case 'c':
                            activator.Bus.Publish(new TimeSpanMessage(DateTime.Now - startupTime)).Wait();
                            break;

                        case 'd':
                            activator.Bus.Publish(new StringMessage("Test String 2")).Wait();
                            break;

                        case 'e':
                            activator.Bus.Publish(new TestMessage("1","2")).Wait();
                            break;


                        case 'f':
                            for (int i = 1; i < 100; i++)
                            {
                                activator.Bus.Publish(new DateTimeMessage(DateTime.Now)).Wait();
                                System.Threading.Thread.Sleep(100);
                            }
                            break;

                        case 'q':
                            goto consideredHarmful;

                        default:
                            Console.WriteLine("There's no option ({0})", keyChar);
                            break;
                    }
                }

            consideredHarmful: ;
                Console.WriteLine("Quitting!");
            }
        }
    }
}
