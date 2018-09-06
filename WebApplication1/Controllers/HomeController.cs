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

namespace WebApplication1.Controllers
{
    public class BasicMessage
    {
        public string Prop { get; set; }
    }

    public class HomeController : Controller
    {
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
            var client = BusClientFactory.CreateDefault(config);

            client.PublishAsync(new BasicMessage { Prop = "Hello, world!" });
            //var client = BusClientFactory.CreateDefault();
            //client.SubscribeAsync<BasicMessage>(async (msg, context) =>
            //{
            //    Console.WriteLine($"Recieved: {msg.Prop}.");

            //});


            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
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
