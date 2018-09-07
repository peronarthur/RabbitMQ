using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.Mvc;
using RawRabbit.Configuration;
using RawRabbit.DependencyInjection.Autofac;
using RawRabbit.vNext;
using WebApplication1.Models;

using Base;
using RabbitMQ.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RawRabbit.Logging;
using ILoggerFactory = Microsoft.Extensions.Logging.ILoggerFactory;
using RawRabbit.vNext.Disposable;
using RawRabbit.Extensions.Client;
using System.Linq;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RawRabbit.Context;
using RawRabbit.Extensions.BulkGet;
using RawRabbit.Extensions.Client;
using RawRabbit.Logging;
using RawRabbit.vNext;



namespace WebApplication1.Controllers
{
    public class TesteMessage
    {
        public string Prop { get; set; }
    }

    public class BasicMessage
    {
        public string Prop { get; set; }
    }

    public class HomeController : Controller
    {



        private static Action<IServiceCollection> AddTestConfig(Action<IServiceCollection> action)
        {
            action = action ?? (collection => { });
            action += collection =>
            {
                var prevRegged = collection
                    .LastOrDefault(c => c.ServiceType == typeof(RawRabbitConfiguration))?
                    .ImplementationFactory(null) as RawRabbitConfiguration;
                if (prevRegged != null)
                {
                    var config = new RawRabbitConfiguration
                    {
                        Username = "xxmjbqcj",
                        Password = "xYFNc0573dICM5qxnMYWmGxo1ey4DEuS",
                        VirtualHost = "xxmjbqcj",
                        Hostnames = { "chimpanzee.rmq.cloudamqp.com" }
                        // more props here.
                    };

                    collection.AddSingleton<RawRabbitConfiguration>(p => config);
                }
            };
            return action;
        }

        public static RawRabbit.Extensions.Client.IBusClient CreateExtendable(Action<IServiceCollection> custom = null)
        {
            custom = AddTestConfig(custom);
            return RawRabbitFactory.Create(custom);
        }

        public IActionResult Index()
        {
            //var builder = new ContainerBuilder();
            //builder.RegisterRawRabbit("amqp://xxmjbqcj:xYFNc0573dICM5qxnMYWmGxo1ey4DEuS@chimpanzee.rmq.cloudamqp.com/xxmjbqcj");
            //var container = builder.Build();

            var config = new RawRabbitConfiguration
            {
                Username = "xxmjbqcj",
                Password = "xYFNc0573dICM5qxnMYWmGxo1ey4DEuS",
                VirtualHost = "xxmjbqcj",
                Hostnames = { "chimpanzee.rmq.cloudamqp.com" }
                // more props here.
            };
            //var client = CreateExtendable();
            var client = BusClientFactory.CreateDefault(config); 

            
            //var bulk = client.GetMessages(cfg => cfg
            //    .ForMessage<BasicMessage>(msg => msg
            //        .FromQueues("basicmessage_webapplication1".ToLower()))
            //    .ForMessage<TesteMessage>(msg => msg
            //        .FromQueues("testemessage_webapplication1")
            //        .GetAll()
            //        .WithNoAck()
            //    ));

            //var a = bulk.GetMessages<BasicMessage>().ToList();
                        
            client.PublishAsync(new TesteMessage { Prop = "Hello, world!" });
            client.PublishAsync(new BasicMessage { Prop = "Hello, world!" });
            //client.PublishAsync(new Base.TesteMessage { Prop = "Hello, world!" }, Guid.NewGuid(), t => t.WithRoutingKey("basicmessage_webapplication1"));
            //var client = BusClientFactory.CreateDefault();
            //client.SubscribeAsync<TesteMessage>(async (msg, context) =>
            //{
            //    Console.WriteLine($"Recieved: {msg.Prop}.");

            //});


            return View();
        }

        public IActionResult About()
        {
            var config = new RawRabbitConfiguration
            {
                Username = "xxmjbqcj",
                Password = "xYFNc0573dICM5qxnMYWmGxo1ey4DEuS",
                VirtualHost = "xxmjbqcj",
                Hostnames = { "chimpanzee.rmq.cloudamqp.com" },
                // more props here.
            };
            var client = BusClientFactory.CreateDefault(config);

            //var client = CreateExtendable();

            //var bulk = client.GetMessages(cfg => cfg
            //    .ForMessage<BasicMessage>(msg => msg
            //        .FromQueues("basicmessage_webapplication1".ToLower()))
            //    .ForMessage<TesteMessage>(msg => msg
            //        .FromQueues("testemessage_webapplication1")
            //        .GetAll()
            //        .WithNoAck()
            //    ));


            Action action = () => client.SubscribeAsync<BasicMessage>(async (msg, context) =>
            {
                //var a = bulk.GetMessages<BasicMessage>().ToList();

                ViewData["Message"] += $"mensagem : {msg.Prop}.";


                Console.WriteLine($"asdfasdfasdfasdf: {msg.Prop}.");
                teste(msg.Prop);
            }, conf => conf.WithQueue(q => q.WithName("basicmessage")));

            Task.Factory.StartNew(() =>
            {
                action();
            });

            client.SubscribeAsync<TesteMessage>(async (msg, context) =>
            {
                ViewData["Message"] = $"Rec23wfasdfasdfieved: {msg.Prop}.";
            }, conf => conf.WithQueue(q => q.WithName("testemessage")));

            return View();
        }

        private void teste(string s)
        {
            for (int i = 0; i < 10000; i++)
            {
                Console.WriteLine($"s: {s}  ------  cont: {i}.");
            }
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
