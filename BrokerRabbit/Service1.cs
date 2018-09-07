using RawRabbit.Configuration;
using RawRabbit.Extensions.BulkGet;
using RawRabbit.Extensions.Client;
using RawRabbit.vNext;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Base;

namespace BrokerRabbit
{
  
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }

        public void OnDebug()
        {
            OnStart(null);
        }

        protected override void OnStart(string[] args)
        {
            System.Diagnostics.Debugger.Launch();

            var config = new RawRabbitConfiguration
            {
                Username = "xxmjbqcj",
                Password = "xYFNc0573dICM5qxnMYWmGxo1ey4DEuS",
                VirtualHost = "xxmjbqcj",
                Hostnames = { "chimpanzee.rmq.cloudamqp.com" },
            };
            
            var client = BusClientFactory.CreateDefault(config);

            client.SubscribeAsync<TesteMessage>(async (msg, context) =>
            {
                Console.WriteLine($"Recieved: {msg.Prop}.");
                

            }, conf => conf.WithQueue(q=> q.WithName("basicmessage_webapplication1")).WithSubscriberId(""));
        }

        protected override void OnStop()
        {
        }
    }
}
