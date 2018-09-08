using Autofac.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RawRabbit.Attributes;
using RawRabbit.Configuration;
using RawRabbit.Configuration.Publish;
using RawRabbit.Context;
using RawRabbit.Extensions.BulkGet;
using RawRabbit.Extensions.Client;
using RawRabbit.vNext;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class UpdateMessage
    {
        public string Loja { get; set; }
        public string Versao { get; set; }
    }

    public class HomeController : Controller
    {

        public int QuantidadeFilas { get { return 4; } }
        //public int CountPublicacoes { get; set; }
        //public int CountLidas { get; set; }
        //public TaskCompletionSource<int> TotalChamadas { get; set; }

        /*
         Passos:
         0 - Colocar as filas para ouvirem as mensagens;
         1 - Salvar no banco a ordem;
         2 - Mandar atualizar;
         3 - Disparar mensagem para uma fila;
         4 - Consumir a ação de uma fila;
         5 - Verificar nos pontos de paradas da atualização se ela precisa parar;
         6 - Realizar a atualização da loja; */

        public IActionResult Index()
        {
            //var client = CreateExtendable();
            var client = BusClientFactory.CreateDefault(RetornaConfiguracaoDasFilas());


            var msg = new UpdateMessage { Loja = "homologacao", Versao = "3.103.00.00" };

            //CountPublicacoes = 1;


            client.PublishAsync(msg);


            return View();
        }

        private RawRabbitConfiguration RetornaConfiguracaoDasFilas()
        {
            return new RawRabbitConfiguration
            {
                Username = "xxmjbqcj",
                Password = "xYFNc0573dICM5qxnMYWmGxo1ey4DEuS",
                VirtualHost = "xxmjbqcj",
                Hostnames = { "chimpanzee.rmq.cloudamqp.com" },
            };
        }

        private Action<IServiceCollection> AddTestConfig(Action<IServiceCollection> action)
        {
            action = action ?? (collection => { });
            action += collection =>
            {
                var prevRegged = RetornaConfiguracaoDasFilas();
                if (prevRegged != null)
                {
                    collection.AddSingleton<RawRabbitConfiguration>(p => prevRegged);
                }

            };
            return action;
        }

        public RawRabbit.Extensions.Disposable.IBusClient CreateExtendable(Action<IServiceCollection> custom = null)
        {
            custom = AddTestConfig(custom);
            return RawRabbitFactory.Create(custom);
        }
        
        private void CriarFilasESubescrever(int numeroFila)
        {
            Func<UpdateMessage, MessageContext, Task> subscribeMethod = async (msg, context) =>
            {
                //CountLidas++;
                //if (CountLidas == CountPublicacoes)
                //{
                //    TotalChamadas.SetResult(CountPublicacoes);
                //}
                Console.WriteLine($"Atualizando loja {msg.Loja} para versão => {msg.Versao}.");
                teste();
            };

            //Action action = () =>
            //{
            //    var client = CreateExtendable();
            //    var bulk = client.GetMessages(cfg => cfg
            //            .ForMessage<UpdateMessage>(msg => msg
            //                .FromQueues($"UpdateMessage_{numeroFila}")
            //                .GetAll()));

            //    foreach (var item in bulk.GetMessages<UpdateMessage>().ToList())
            //    {
            //        teste(item.Message.Loja);
            //    }                
            //};

            Action action = () => BusClientFactory.CreateDefault(null, AddTestConfig(null))
                                              .SubscribeAsync(subscribeMethod, conf => conf.WithQueue(q => q.WithName($"UpdateMessage_{numeroFila}"))
                                                                                           .WithPrefetchCount(1));

            Task.Factory.StartNew(() =>
            {
                action();
            });
        }


        private void teste()
        {
            Thread.Sleep(5000);
            Console.WriteLine("Iniciando atualização da loja");
        }

        private void teste(string nomeLoja)
        {
            Thread.Sleep(5000);
            Console.WriteLine($"Iniciando atualização da {nomeLoja}");
        }

        public IActionResult About()
        {
            for (int i = 1; i <= QuantidadeFilas; i++)
            {
                CriarFilasESubescrever(i);
            }

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
