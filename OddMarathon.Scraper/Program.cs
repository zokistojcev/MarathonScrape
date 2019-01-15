
using System.Threading.Tasks;
using Unity;
using Unity.Lifetime;
using System.Data.Entity;
using System.Threading;
using System.Collections.Generic;
using System;

namespace OddMarathon.Scraper
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }

        static async Task MainAsync(string[] args)
        {
            IUnityContainer container = new UnityContainer();
            DependencyInjection.RegisterDependency(container);

            

            var oddRequests = container.Resolve<OddRequests>();

            //oddRequests.Kopacka();
            //Thread.Sleep(3000);

            await oddRequests.GetFootballOddsFinal();
            Thread.Sleep(7000);

            await oddRequests.GetTennisOddsFinal();
            Thread.Sleep(7000);

        }
        
    }
}
