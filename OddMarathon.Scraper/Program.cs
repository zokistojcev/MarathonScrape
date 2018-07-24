using OddMarathon.Dal.DataAccess;
using OddMarathon.Dal.Repositories;
using OddMarathon.Dal.Repositories.OddsRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unity;

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
            container.RegisterType<IOddsRepository, OddsRepository>();
            container.RegisterType<OddMarathonContext>();
            container.RegisterType<OddRequests>();

            var oddRequests = container.Resolve<OddRequests>();
            await oddRequests.GetTennisOdds();
            //await oddRequests.GetFootballsOdds();
        }
    }
}
