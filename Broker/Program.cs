using RawRabbit.Configuration;
using RawRabbit.vNext;
using System;

namespace Broker
{
    class Program
    {
        public class BasicMessage
        {
            public string Prop { get; set; }
        }

        static void Main(string[] args)
        {

            var config = new RawRabbitConfiguration
            {
                Username = "xxmjbqcj",
                Password = "xYFNc0573dICM5qxnMYWmGxo1ey4DEuS",
                VirtualHost = "xxmjbqcj",
                Hostnames = { "chimpanzee.rmq.cloudamqp.com" }
                // more props here.
            };
            var client = BusClientFactory.CreateDefault(config);

            //client.PublishAsync(new BasicMessage { Prop = "Hello, world!" });
            //var client = BusClientFactory.CreateDefault();
            //var a = new thre


            client.SubscribeAsync<BasicMessage>(async (msg, context) =>
            {
                Console.WriteLine($"Recieved: {msg.Prop}.");
                Console.ReadKey();

            });


        }
    }
}
