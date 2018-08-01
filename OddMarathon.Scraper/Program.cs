
using System.Threading.Tasks;
using Unity;
using Unity.Lifetime;
using System.Data.Entity;

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

            var urlFootball = "https://www.marathonbet.com/en/popular/Football/?menu=11";
            var urlChess = "https://www.marathonbet.com/en/betting/Chess/?menu=140628";
            var urlCricket = "https://www.marathonbet.com/en/betting/Cricket/?menu=8";
            var urlBoxing = "https://www.marathonbet.com/en/betting/Boxing/?menu=7";
            var urlTennis = "https://www.marathonbet.com/en/betting/Tennis/?menu=2398";
            var urlBasketball = "https://www.marathonbet.com/en/betting/Basketball/?menu=6";
            var urlMMA = "https://www.marathonbet.com/en/betting/MMA/?menu=439050";

            var urlBaseballLive = "https://www.marathonbet.com/en/live/120866";



            var tttb = await oddRequests.GetTennisOdds(urlTennis);
            var tttfb = await oddRequests.GetTennisOdds(urlBasketball);






            //var basketball = await oddRequests.GetTennisOdds(urlBasketball);
            //await oddRequests.GetTennisOdds(urlBoxing);
            //await oddRequests.GetTennisOdds(urlMMA);
            //await oddRequests.GetFootballsOdds(urlFootball);
            //await oddRequests.GetFootballsOdds(urlChess);
            //await oddRequests.GetTennisOdds(urlBaseballLive);

            //await oddRequests.GetFootballsOdds(urlCricket);

        }
    }
}
